using Blog.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> QueryAllPostsAsync(string? title, string? author, DateTime? startDate, DateTime? endDate);
        Task<BlogPost?> GetPostRecordByIdAsync(int id);
        Task AddPostRecordAsync(BlogPost post);
        Task UpdatePostRecordAsync(BlogPost post);
        Task DeletePostRecordAsync(int id);
    }
}
