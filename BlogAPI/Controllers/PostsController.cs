using AutoMapper;
using BlogAPI.DTOs;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostsController(IPostRepository postRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            var posts = await _postRepository.GetAllPostsAsync();
            return Ok(_mapper.Map<IEnumerable<PostDto>>(posts));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if(post == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostDto>(post));
        }

        [HttpPost]
        [SwaggerFileUpload]
        public async Task<ActionResult<PostDto>> CreatePost([FromForm] PostCreateDto postCreateDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _mapper.Map<Post>(postCreateDto);

            // Handle image upload
            if (postCreateDto.ImageFile != null && postCreateDto.ImageFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(postCreateDto.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postCreateDto.ImageFile.CopyToAsync(fileStream);
                }

                // Store relative path in database
                post.ImageUrl = $"/uploads/{uniqueFileName}";
            }

            // Handle video upload (new)
            if (postCreateDto.VideoFile != null)
            {
                post.VideoUrl = await SaveFile(postCreateDto.VideoFile, ["video/mp4", "video/quicktime"], maxSizeMB: 50);
            }

            var createdPost = await _postRepository.AddPostAsync(post, userId);
            var postDto = _mapper.Map<PostDto>(createdPost);

            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }

        [HttpPut("{id}")]
        //[SwaggerFileUpload]
        public async Task<IActionResult> UpdatePost(int id, PostUpdateDto postUpdateDto)
        {
            if(!await _postRepository.PostExists(id))
            {
                return NotFound();
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(post.CreatedById != userId)
            {
                return Forbid();
            }

            _mapper.Map(postUpdateDto, post);
            await _postRepository.UpdatePostAsync(post, userId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if(post  == null)
            {
                return NotFound();
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (post.CreatedById != userId)
            {
                return Forbid();
            }

            await _postRepository.DeletePostAsync(id);
            return NoContent();
        }

        private async Task<string?> SaveFile(IFormFile file, string[] allowedMimeTypes, int maxSizeMB = 10)
        {
            // Validate file type
            if (!allowedMimeTypes.Contains(file.ContentType))
            {
                throw new BadHttpRequestException($"Invalid file type. Allowed: {string.Join(", ", allowedMimeTypes)}");
            }

            // Validate size
            if (file.Length > maxSizeMB * 1024 * 1024)
            {
                throw new BadHttpRequestException($"File too large (max {maxSizeMB}MB)");
            }

            var uploadsDir = Path.Combine("wwwroot", "uploads");
            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, uniqueName);

            Directory.CreateDirectory(uploadsDir);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{uniqueName}";
        }

    }
}
