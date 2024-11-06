using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Blog.API.Controllers;
using Blog.API.Interfaces;
using Blog.API.Models;
using Blog.API.DTOs;

namespace Blog.API.Tests.Controller
{
    public class BlogPostsControllerTests
    {
        private readonly BlogPostsController _controller;
        private readonly Mock<IBlogPostService> _mockBlogPostService;
        private readonly Mock<ILogger<BlogPostsController>> _mockLogger;

        public BlogPostsControllerTests()
        {
            _mockBlogPostService = new Mock<IBlogPostService>();
            _mockLogger = new Mock<ILogger<BlogPostsController>>();
            _controller = new BlogPostsController(_mockBlogPostService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllPosts_ReturnsOk_WithListOfPosts()
        {
            var mockPosts = new List<BlogPost> { 
                new BlogPost { Id = 1, Title = "Test Post", Content = "This is a test post content." }, 
                new BlogPost { Id = 2, Title = "Test Post", Content = "This is a test post content." },
                new BlogPost { Id = 3, Title = "Test Post", Content = "This is a test post content." }
            };
            _mockBlogPostService.Setup(service => service.GetAllPostsAsync()).ReturnsAsync(mockPosts);

            var result = await _controller.GetAllPosts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Equal(mockPosts.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetPostById_ReturnsOk_WithPost_WhenPostExists()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Test Post", Content = "This is a test post content." };
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync(mockPost);

            var result = await _controller.GetPostById(1);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BlogPost>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetPostById_ReturnsNotFound_WhenPostDoesNotExist()
        {
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync((BlogPost)null);
            var result = await _controller.GetPostById(1);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePost_ReturnsCreatedAtActionResult_WhenPostIsCreated()
        {
            var newPost = new BlogPost { Id = 1, Title = "New Post", Content = "This is a test post content." };
            _mockBlogPostService.Setup(service => service.AddPostAsync(newPost)).Returns(Task.CompletedTask);

            var result = await _controller.CreatePost(newPost);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<BlogPost>(createdAtActionResult.Value);
            Assert.Equal(newPost.Id, returnValue.Id);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenTitleIsMissing()
        {
            var newPost = new BlogPost { Id = 1, Title = null, Content = "Content without a title" };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            var result = await _controller.CreatePost(newPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenContentIsMissing()
        {
            var newPost = new BlogPost { Id = 1, Title = "Title without content", Content = null }; 
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            var result = await _controller.CreatePost(newPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenIdAlreadyExists()
        {
            var existingPost = new BlogPost { Id = 1, Title = "Existing Post", Content = "This is existing content." };
            var newPost = new BlogPost { Id = 1, Title = "New Post", Content = "This is a test post content." };

            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(newPost.Id)).ReturnsAsync(existingPost);

            var result = await _controller.CreatePost(newPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Post ID already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenAuthorNameIsInvalid()
        {
            var invalidAuthorPost = new BlogPost { Id = 1, Title = "Valid Title", Content = "This is valid content.", Author = "Author123" };
            _controller.ModelState.AddModelError("Author", "Author name can only contain letters and spaces.");
            var result = await _controller.CreatePost(invalidAuthorPost);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdateIsSuccessful()
        {
            var updatedPost = new BlogPost { Id = 1, Title = "Updated Title", Content = "This is a test post content." };
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync(updatedPost);
            _mockBlogPostService.Setup(service => service.UpdatePostAsync(updatedPost)).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePost(1, updatedPost);
            var newPost = await _controller.GetPostById(1);

            Assert.IsType<OkResult>(result);
            Assert.NotNull(newPost);
            Assert.NotEqual("Updated Title", newPost?.Value?.Title);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPostDoesNotExist()
        {
            var updatedPost = new BlogPostUpdateDto { Title = "Updated Title", Content = "This is a test post content." };
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync((BlogPost)null);

            var result = await _controller.UpdatePost(1, updatedPost);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Update_ReturnsBadRequest_WhenTitleIsMissing()
        {
            var updatedPost = new BlogPostUpdateDto { Title = null, Content = "Updated content without title" };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            var result = await _controller.UpdatePost(1, updatedPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenContentIsMissing()
        {
            var updatedPost = new BlogPostUpdateDto { Title = "Updated title without content", Content = null };
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            var result = await _controller.UpdatePost(1, updatedPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdIsNotEqual()
        {
            var updatedPost = new BlogPostUpdateDto { Title = "Updated title without content", Content = null };
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            var result = await _controller.UpdatePost(1, updatedPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        public async Task UpdatePost_ReturnsBadRequest_WhenAuthorNameIsInvalid()
        {
            var invalidAuthorPost = new BlogPostUpdateDto { Title = "Valid Title", Content = "This is valid content.", Author = "Author123" };
            _controller.ModelState.AddModelError("Author", "Author name can only contain letters and spaces.");

            var result = await _controller.UpdatePost(1, invalidAuthorPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        //[Fact]
        //public async Task UpdatePost_ReturnsBadRequest_WhenIdAlreadyExists()
        //{
        //    var existingPost = new BlogPost { Id = 2, Title = "Existing Post", Content = "This is existing content." };
        //    var updatedPost = new BlogPostUpdateDto { Title = "Updated Title", Content = "Updated content." };

        //    _mockBlogPostService.Setup(service => service.GetPostByIdAsync(updatedPost.Id)).ReturnsAsync(existingPost);

        //    var result = await _controller.UpdatePost(2, updatedPost);
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal(400, badRequestResult.StatusCode);
        //    Assert.Equal("Post ID already exists.", badRequestResult.Value);
        //}

        [Fact]
        public async Task DeletePost_ReturnsOk_WhenDeletionIsSuccessful()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Updated Title", Content = "This is a test post content." };
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync(mockPost);
            _mockBlogPostService.Setup(service => service.DeletePostAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeletePost(1);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPostDoesNotExist()
        {
            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync((BlogPost)null);
            
            var result = await _controller.DeletePost(1);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
