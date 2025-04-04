using BlogAPI.Models;

namespace BlogAPI.Interfaces
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int  id);
        Task<Post> AddPostAsync(Post post, string userId);
        Task<Post> UpdatePostAsync(Post post, string userId);
        Task DeletePostAsync(int id);
        Task<bool> PostExists(int id);

    }
}
