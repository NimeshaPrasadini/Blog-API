using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string? ImageUrl { get; set; }  // Stores the file path/URL
        public string? VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("UpdatedBy")]
        public string? UpdatedById { get; set; }
        public virtual ApplicationUser? UpdatedBy { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
