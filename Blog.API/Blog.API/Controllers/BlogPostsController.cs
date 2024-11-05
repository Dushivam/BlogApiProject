using Microsoft.AspNetCore.Mvc;
using Blog.API.Interfaces;
using Blog.API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetAll()
        {
            var posts = await _blogPostService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetById(int id)
        {
            var post = await _blogPostService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<BlogPost>> Create(BlogPost post)
        {
            if (post == null)
            {
                return BadRequest("Post cannot be null.");
            }

            await _blogPostService.AddPostAsync(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BlogPost post)
        {
            if (id != post.Id)
            {
                return BadRequest("Post ID mismatch.");
            }

            var existingPost = await _blogPostService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            await _blogPostService.UpdatePostAsync(post);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _blogPostService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            await _blogPostService.DeletePostAsync(id);
            return NoContent();
        }
    }
}
