using BlogAPI.Data;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> AddCommentAsync(Comment comment, string userId)
        {
            comment.UserId = userId;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;

        }

        public async Task<Comment> UpdateCommentAsync(Comment comment, string userId)
        {
            comment.UserId = userId;
            comment.UpdatedAt = DateTime.UtcNow;
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return comment;

        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CommentExists(int id)
        {
            return await _context.Comments.AnyAsync(e => e.Id == id);
        }
    }
}
