using Blog.API.Interfaces;
using Blog.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _postRepository;

        public BlogPostService(IBlogPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllAsync();
        }
        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _postRepository.GetByIdAsync(id);
        }
        public async Task AddPostAsync(BlogPost post)
        {
            await _postRepository.AddAsync(post);
        }
        public async Task UpdatePostAsync(BlogPost post)
        {
            await _postRepository.UpdateAsync(post);
        }
        public async Task DeletePostAsync(int id)
        {
            await _postRepository.DeleteAsync(id);
        }
    }
}
