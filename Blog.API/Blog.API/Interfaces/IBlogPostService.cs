using Blog.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API.Interfaces
{
    public interface IBlogPostService
    {
        Task<IEnumerable<BlogPost>> GetAllPostsAsync(string? title, string? author, DateTime? startDate, DateTime? endDate);
        Task<BlogPost?> GetPostByIdAsync(int id);
        Task AddPostAsync(BlogPost post);
        Task UpdatePostAsync(BlogPost post);
        Task DeletePostAsync(int id);
    }
}
