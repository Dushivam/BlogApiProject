using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models
{
    public class BlogPost
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
    }

}
