namespace BlogAPI.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set;}
    }

    public class PostCreateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class PostUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
