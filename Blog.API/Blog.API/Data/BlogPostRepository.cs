using Blog.API.Interfaces;
using Blog.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<BlogPostRepository> _logger;
        public BlogPostRepository(BlogDbContext context, ILogger<BlogPostRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BlogPost>> QueryAllPostsAsync(string? title = null,string? author = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Posts.AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    _logger.LogInformation("Retrieving blog posts from the database with filter title = {title}", title);
                    query = query.Where(post => post.Title.Contains(title));
                }

                if (!string.IsNullOrEmpty(author))
                {
                    _logger.LogInformation("Retrieving blog posts from the database with filter author = {author}", author);
                    query = query.Where(post => post.Author.Contains(author));
                }

                if (startDate.HasValue)
                {
                    _logger.LogInformation("Retrieving blog posts from the database with filter startDate = {date}", startDate.Value);
                    query = query.Where(post => post.PublishedDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    _logger.LogInformation("Retrieving blog posts from the database with filter endDate = {date}", endDate.Value);
                    query = query.Where(post => post.PublishedDate <= endDate.Value);
                }

                var posts = await query.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} posts from the database", posts.Count);
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog posts with filters from the database");
                throw;
            }
        }

        public async Task<BlogPost?> GetPostRecordByIdAsync(int id)
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

        public async Task AddPostRecordAsync(BlogPost post)
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

        public async Task UpdatePostRecordAsync(BlogPost post)
        {
            _logger.LogInformation("Updating blog post with ID {Id} in the database", post.Id);
            try
            {
                var existingPost = await _context.Posts.FindAsync(post.Id);
                if (existingPost != null)
                {
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.Author = post.Author;
                    existingPost.PublishedDate = post.PublishedDate;

                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("Successfully updated blog post with ID {Id} in the database", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating blog post with ID {Id} in the database", post.Id);
                throw;
            }
        }

        public async Task DeletePostRecordAsync(int id)
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
