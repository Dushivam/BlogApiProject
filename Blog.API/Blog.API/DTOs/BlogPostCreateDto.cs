using System.ComponentModel.DataAnnotations;

namespace Blog.API.DTOs
{
    public class BlogPostCreateDto
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Author name can only contain letters and spaces.")]
        public string Author { get; set; }
    }
}
