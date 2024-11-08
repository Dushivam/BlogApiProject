using System.ComponentModel.DataAnnotations;

namespace Blog.API.DTOs
{
    public class BlogPostUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Author name can only contain letters and spaces.")]
        public string? Author { get; set; }
    }

}
