using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        public string? DisplayName { get; set; }
    }
}
