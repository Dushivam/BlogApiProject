using Blog.API.Interfaces;
using Blog.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _postRepository;
        private readonly ILogger<BlogPostService> _logger;
        public BlogPostService(IBlogPostRepository postRepository, ILogger<BlogPostService> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync(string? title = null, string? author = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            _logger.LogInformation("Retrieving blog posts with filters {title}, {author}, {startdate}, {endDate}", title, author, startDate, endDate);

            try
            {
                var posts = await _postRepository.QueryAllPostsAsync(title, author, startDate, endDate);
                _logger.LogInformation("Successfully retrieved {Count} posts", posts.Count());
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog posts");
                throw;
            }
        }


        public async Task<BlogPost?> GetPostByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving blog post with ID {Id}", id);
            try
            {
                var post = await _postRepository.GetPostRecordByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved blog post with ID {Id}", id);
                }
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog post with ID {Id}", id);
                throw;
            }
        }

        public async Task AddPostAsync(BlogPost post)
        {
            _logger.LogInformation("Adding a new blog post");
            if (post == null)
            {
                _logger.LogWarning("Attempted to add a null blog post");
                throw new ArgumentNullException(nameof(post), "Post cannot be null");
            }

            try
            {
                await _postRepository.AddPostRecordAsync(post);
                _logger.LogInformation("Successfully added blog post with ID {Id}", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new blog post");
                throw;
            }
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            _logger.LogInformation("Updating blog post with ID {Id}", post.Id);
            if (post == null)
            {
                _logger.LogWarning("Attempted to update a null blog post");
                throw new ArgumentNullException(nameof(post), "Post cannot be null");
            }

            try
            {
                await _postRepository.UpdatePostRecordAsync(post);
                _logger.LogInformation("Successfully updated blog post with ID {Id}", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating blog post with ID {Id}", post.Id);
                throw;
            }
        }

        public async Task DeletePostAsync(int id)
        {
            _logger.LogInformation("Deleting blog post with ID {Id}", id);
            try
            {
                await _postRepository.DeletePostRecordAsync(id);
                _logger.LogInformation("Successfully deleted blog post with ID {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting blog post with ID {Id}", id);
                throw;
            }
        }
    }
}
