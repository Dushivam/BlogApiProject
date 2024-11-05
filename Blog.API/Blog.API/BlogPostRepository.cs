using Blog.API.Interfaces;
using Blog.API.Models;
using Blog.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        private readonly ILogger _logger;
        public BlogPostRepository(BlogDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all blog posts from the database");
            try
            {
                var posts = await _context.Posts.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} posts from the database", posts.Count);
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all blog posts from the database");
                throw;
            }
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving blog post with ID {Id} from the database", id);
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found in the database", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved blog post with ID {Id} from the database", id);
                }
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog post with ID {Id} from the database", id);
                throw;
            }
        }

        public async Task AddAsync(BlogPost post)
        {
            _logger.LogInformation("Adding a new blog post to the database");
            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully added blog post with ID {Id} to the database", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new blog post to the database");
                throw;
            }
        }

        public async Task UpdateAsync(BlogPost post)
        {
            _logger.LogInformation("Updating blog post with ID {Id} in the database", post.Id);
            try
            {
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated blog post with ID {Id} in the database", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating blog post with ID {Id} in the database", post.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting blog post with ID {Id} from the database", id);
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found in the database", id);
                    return;
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted blog post with ID {Id} from the database", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting blog post with ID {Id} from the database", id);
                throw;
            }
        }
    }
}
