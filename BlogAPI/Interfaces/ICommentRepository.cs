using BlogAPI.Models;

namespace BlogAPI.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<Comment> GetCommentByIdAsync(int id);
        Task<Comment> AddCommentAsync(Comment comment, string userId);
        Task<Comment> UpdateCommentAsync(Comment comment, string userId);
        Task DeleteCommentAsync(int id);
        Task<bool> CommentExists(int id);

    }
}
