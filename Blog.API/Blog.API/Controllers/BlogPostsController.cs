﻿using Microsoft.AspNetCore.Mvc;
using Blog.API.Interfaces;
using Blog.API.Models;
using Blog.API.DTOs;

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
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetAllPosts()
        {
            _logger.LogInformation("Getting all blog posts");
            try
            {
                var posts = await _blogPostService.GetAllPostsAsync();
                _logger.LogInformation("Successfully retrieved {Count} posts", posts.Count());
                return Ok(posts);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "An database error occurred while creating a new blog post");
                return StatusCode(500, $"Internal server error. {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all blog posts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetPostById(int id)
        {
            _logger.LogInformation("Getting blog post with ID {Id}", id);
            try
            {
                var post = await _blogPostService.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                    return NotFound();
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
        public async Task<ActionResult<BlogPost>> CreatePost(BlogPostCreateDto postDto)
        {
            _logger.LogInformation("Creating a new blog post");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid blog post data provided");
                return BadRequest(ModelState);
            }

            // Check if a post with the same ID already exists
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


        //[HttpPost]
        //public async Task<ActionResult<BlogPost>> CreatePost(BlogPost post)
        //{
        //    _logger.LogInformation("Creating a new blog post");

        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogWarning("Invalid blog post data provided");
        //        return BadRequest(ModelState);
        //    }

        //    var existingPost = await _blogPostService.GetPostByIdAsync(post.Id);
        //    if (existingPost != null)
        //    {
        //        _logger.LogWarning("Post ID already exists.");
        //        return BadRequest("Post ID already exists.");
        //    }

        //    try
        //    {
        //        await _blogPostService.AddPostAsync(post);
        //        _logger.LogInformation("Successfully created blog post with ID {Id}", post.Id);
        //        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        //    }
        //    catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        //    {
        //        _logger.LogError(dbEx, "A database error occurred while creating a new blog post");
        //        return StatusCode(500, $"Internal server error. {dbEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while creating a new blog post");
        //        return StatusCode(500, $"Internal server error. {ex.Message}");
        //    }
        //}

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
                    return NotFound();
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
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePost(int id, BlogPost post)
        //{
        //    _logger.LogInformation("Updating blog post with ID {Id}", id);

        //    if (id != post.Id)
        //    {
        //        _logger.LogWarning("Post ID mismatch. Provided ID: {Id}, Post ID: {PostId}", id, post.Id);
        //        return BadRequest("Post ID mismatch.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogWarning("Invalid blog post data provided");
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var existingPost = await _blogPostService.GetPostByIdAsync(id);
        //        if (existingPost == null)
        //        {
        //            _logger.LogWarning("Blog post with ID {Id} not found", id);
        //            return NotFound();
        //        }

        //        var duplicatePost = await _blogPostService.GetPostByIdAsync(post.Id);
        //        if (duplicatePost != null && duplicatePost.Id != id)
        //        {
        //            _logger.LogWarning("Post ID already exists.");
        //            return BadRequest("Post ID already exists.");
        //        }

        //        await _blogPostService.UpdatePostAsync(post);
        //        _logger.LogInformation("Successfully updated blog post with ID {Id}", id);
        //        return Ok();
        //    }
        //    catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        //    {
        //        _logger.LogError(dbEx, "A database error occurred while updating the blog post");
        //        return StatusCode(500, $"Internal server error. {dbEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while updating blog post with ID {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            _logger.LogInformation("Deleting blog post with ID {Id}", id);
            try
            {
                var post = await _blogPostService.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Blog post with ID {Id} not found", id);
                    return NotFound();
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
