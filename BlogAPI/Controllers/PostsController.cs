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
        public async Task<ActionResult<PostDto>> CreatePost(PostCreateDto postCreateDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = _mapper.Map<Post>(postCreateDto);

            var createdPost = await _postRepository.AddPostAsync(post, userId);
            var postDto = _mapper.Map<PostDto>(createdPost);

            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }

        [HttpPut("{id}")]
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

    }
}
