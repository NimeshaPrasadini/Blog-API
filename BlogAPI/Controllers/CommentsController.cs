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
    [Route("api/posts/{postId}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentsController(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int postId)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return NotFound("Post not found");
            }

            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            return Ok(_mapper.Map<IEnumerable<CommentDto>>(comments));
        }

        [HttpGet("{id}", Name = "GetComment")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentDto>> GetComment(int postId, int id)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return NotFound("Post not found");
            }

            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment == null || comment.PostId != postId)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommentDto>(comment));
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(int postId, CommentCreateDto commentCreateDto)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return NotFound("Post not found");
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = _mapper.Map<Comment>(commentCreateDto);
            comment.PostId = postId;

            var createdComment = await _commentRepository.AddCommentAsync(comment, userId);
            var commentDto = _mapper.Map<CommentDto>(createdComment);

            return CreatedAtRoute("GetComment", new { postId, id = commentDto.Id }, commentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int postId, int id, CommentUpdateDto commentUpdateDto)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return NotFound("Post not found");
            }

            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment == null || comment.PostId != postId)
            {
                return NotFound();
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            _mapper.Map(commentUpdateDto, comment);
            await _commentRepository.UpdateCommentAsync(comment, userId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int postId, int id)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return NotFound("Post not found");
            }

            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment == null || comment.PostId != postId)
            {
                return NotFound();
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            await _commentRepository.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}
