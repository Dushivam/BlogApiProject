using Microsoft.AspNetCore.Mvc;
using Blog.API.Interfaces;
using Blog.API.Models;
using Blog.API.DTOs;
using NSwag.Annotations;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        [HttpGet]
        [OpenApiOperation(summary: "This endpoint returns all blog posts stored in the system.", description: "Returns an array of blog posts. Each blog post includes details such as the ID, title, content, author, and published date. Supports optional filters by title, author, and published date range.")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetAllPosts(string? title = null, string? author = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            _logger.LogInformation("Retrieving all blog posts.");

            try
            {
                var posts = await _blogPostService.GetAllPostsAsync(title, author, startDate, endDate);
                _logger.LogInformation("Successfully retrieved {Count} posts", posts.Count());
                return Ok(posts);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "A database error occurred while retrieving blog posts");
                return StatusCode(500, $"Internal server error. {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all blog posts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [OpenApiOperation(summary: "This endpoint returns a single blog post based on the provided ID.", "")]

        public async Task<ActionResult<BlogPost>> GetPostById(int id)
        {
            _logger.LogInformation("Getting blog post with ID {Id}", id);
            try
            {
                var post = await _blogPostService.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                    return NotFound(new
                    {
                        message = $"Blog post with ID {id} not found.",
                        status = 404
                    });
                }
                _logger.LogInformation("Successfully retrieved blog post with ID {Id}", id);
                return Ok(post);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "An database error occurred while creating a new blog post");
                return StatusCode(500, $"Internal server error. {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog post with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [OpenApiOperation(summary: "This endpoint allows clients to create a new blog post by providing required data.", "")]

        public async Task<ActionResult<BlogPost>> CreatePost(BlogPostCreateDto postDto)
        {
            _logger.LogInformation("Creating a new blog post");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid blog post data provided");
                return BadRequest(ModelState);
            }

            var existingPost = await _blogPostService.GetPostByIdAsync(postDto.Id);
            if (existingPost != null)
            {
                _logger.LogWarning("Post ID already exists.");
                return BadRequest("Post ID already exists.");
            }

            try
            {
                var post = new BlogPost
                {
                    Id = postDto.Id,
                    Title = postDto.Title,
                    Content = postDto.Content,
                    Author = postDto.Author,
                    PublishedDate = DateTime.Now
                };

                await _blogPostService.AddPostAsync(post);
                _logger.LogInformation("Successfully created blog post with ID {Id}", post.Id);
                return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "A database error occurred while creating a new blog post");
                return StatusCode(500, $"Internal server error. {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new blog post");
                return StatusCode(500, $"Internal server error. {ex.Message}");
            }
        }

        [OpenApiOperation(summary: "This endpoint allows clients to update an existing blog post by providing new values for title, content, and author.", "")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, BlogPostUpdateDto postDto)
        {
            _logger.LogInformation("Updating blog post with ID {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid blog post data provided");
                return BadRequest(ModelState);
            }

            try
            {
                var existingPost = await _blogPostService.GetPostByIdAsync(id);
                if (existingPost == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                    return NotFound(new
                    {
                        message = $"Blog post with ID {id} not found.",
                        status = 404
                    });
                }

                existingPost.Title = postDto.Title;
                existingPost.Content = postDto.Content;
                existingPost.Author = postDto.Author;

                await _blogPostService.UpdatePostAsync(existingPost);

                _logger.LogInformation("Successfully updated blog post with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating blog post with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [OpenApiOperation(summary: "This endpoint allows clients to delete a blog post with the specified ID from the system.", "")]
        public async Task<IActionResult> DeletePost(int id)
        {
            _logger.LogInformation("Deleting blog post with ID {Id}", id);
            try
            {
                var post = await _blogPostService.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                    return NotFound(new
                    {
                        message = $"Blog post with ID {id} not found.",
                        status = 404
                    });
                }

                await _blogPostService.DeletePostAsync(id);
                _logger.LogInformation("Successfully deleted blog post with ID {Id}", id);
                return Ok();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "An database error occurred while creating a new blog post");
                return StatusCode(500, $"Internal server error. {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting blog post with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
