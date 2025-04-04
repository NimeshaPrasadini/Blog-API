namespace BlogAPI.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}
        public string User { get; set; }
        public int PostId { get; set; }
    }

    public class CommentCreateDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
    }

    public class CommentUpdateDto
    {
        public string Content { get; set; }
    }
}
