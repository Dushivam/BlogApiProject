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
                new BlogPost { Id = 1, Title = "Journey to the Center of the Earth", Content = "Discover the marvels beneath the Earth's crust and what mysteries lie within.", Author = "Jules Verne", PublishedDate = DateTime.Now.AddDays(-10) },
                new BlogPost { Id = 2, Title = "Mastering the Art of French Cooking", Content = "Learn the techniques that transform simple ingredients into gourmet meals.", Author = "Julia Child", PublishedDate = DateTime.Now.AddDays(-5) },
                new BlogPost { Id = 3, Title = "The Future of Artificial Intelligence", Content = "AI is transforming our world – here’s what the future might hold.", Author = "John McCarthy", PublishedDate = DateTime.Now }
            };

            _mockBlogPostService.Setup(service => service.GetAllPostsAsync(null, null, null, null)).ReturnsAsync(mockPosts);
            var result = await _controller.GetAllPosts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Equal(mockPosts.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetAllPosts_FiltersByTitle_ReturnsMatchingPosts()
        {
            var mockPosts = new List<BlogPost> {
                new BlogPost { Id = 1, Title = "Journey to the Center of the Earth", Content = "Content about the Earth's crust.", Author = "Jules Verne", PublishedDate = DateTime.Now }
            };

            _mockBlogPostService.Setup(service => service.GetAllPostsAsync("Journey", null, null, null)).ReturnsAsync(mockPosts);

            var result = await _controller.GetAllPosts(title: "Journey");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("Journey to the Center of the Earth", returnValue[0].Title);
        }


        [Fact]
        public async Task GetAllPosts_FiltersByAuthor_ReturnsMatchingPosts()
        {
            var mockPosts = new List<BlogPost> {
                new BlogPost { Id = 2, Title = "Mastering the Art of French Cooking", Content = "Learn gourmet techniques.", Author = "Julia Child", PublishedDate = DateTime.Now }
            };

            _mockBlogPostService.Setup(service => service.GetAllPostsAsync(null, "Julia Child", null, null)).ReturnsAsync(mockPosts);

            var result = await _controller.GetAllPosts(author: "Julia Child");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("Julia Child", returnValue[0].Author);
        }

        [Fact]
        public async Task GetAllPosts_FiltersByDateRange_ReturnsMatchingPosts()
        {
            var mockPosts = new List<BlogPost> {
                new BlogPost { Id = 3, Title = "The Future of Artificial Intelligence", Content = "AI advancements.", Author = "John McCarthy", PublishedDate = DateTime.Now.AddDays(-1) }
            };

            var startDate = DateTime.Now.AddDays(-2);
            var endDate = DateTime.Now;

            _mockBlogPostService.Setup(service => service.GetAllPostsAsync(null, null, startDate, endDate)).ReturnsAsync(mockPosts);

            var result = await _controller.GetAllPosts(startDate: startDate, endDate: endDate);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("The Future of Artificial Intelligence", returnValue[0].Title);
        }

        [Fact]
        public async Task GetPostById_ReturnsOk_WithPost_WhenPostExists()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Exploring the World of Quantum Physics", Content = "Quantum physics is strange and wonderful – a fascinating world awaits!." };
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
            var newPostDto = new BlogPostCreateDto
            {
                Title = "Exploring the World of Quantum Physics",
                Content = "Quantum physics is strange and wonderful – a fascinating world awaits!",
                Author = "Rob Stark"
            };

            _mockBlogPostService.Setup(service => service.AddPostAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);

            var result = await _controller.CreatePost(newPostDto);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<BlogPost>(createdAtActionResult.Value);
            Assert.Equal(newPostDto.Title, returnValue.Title);
            Assert.Equal(newPostDto.Content, returnValue.Content);
            Assert.Equal(newPostDto.Author, returnValue.Author);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenTitleIsMissing()
        {
            var newPostDto = new BlogPostCreateDto
            {
                Id = 1,
                Title = null,
                Content = "Manage your time better with these actionable strategies.",
                Author = "Wayne Rooney"
            };

            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            var result = await _controller.CreatePost(newPostDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenContentIsMissing()
        {
            var newPost = new BlogPostCreateDto { Id = 1, Title = "Time Management", Content = null, Author = "James Milner" }; 
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            var result = await _controller.CreatePost(newPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenIdAlreadyExists()
        {
            var existingPost = new BlogPost { Id = 1, Title = "Existing Post", Content = "This is existing content.", Author = "Nicolas Jackson" };
            var newPostDto = new BlogPostCreateDto
            {
                Id = 1,
                Title = "New Post",
                Content = "This is a test post content.",
                Author = "Adam Smith"
            };

            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(existingPost.Id)).ReturnsAsync(existingPost);

            var result = await _controller.CreatePost(newPostDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Post ID already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenAuthorNameIsInvalid()
        {
            var invalidAuthorPost = new BlogPostCreateDto { Id = 1, Title = "Writing for Impact: Crafting Compelling Stories", Content = "Learn the art of storytelling to engage and inspire your readers.", Author = "Elizabeth1498" };
            _controller.ModelState.AddModelError("Author", "Author name can only contain letters and spaces.");
            var result = await _controller.CreatePost(invalidAuthorPost);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            var existingPost = new BlogPost { Id = 1, Title = "Original Title", Content = "This is the original content." };
            var updatedPostDto = new BlogPostUpdateDto { Title = "Updated Title", Content = "This is the updated post content.", Author = "Updated Author" };

            _mockBlogPostService.Setup(service => service.GetPostByIdAsync(1)).ReturnsAsync(existingPost);
            _mockBlogPostService.Setup(service => service.UpdatePostAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePost(1, updatedPostDto);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Title", existingPost.Title);
            Assert.Equal("This is the updated post content.", existingPost.Content);
            Assert.Equal("Updated Author", existingPost.Author);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPostDoesNotExist()
        {
            var updatedPost = new BlogPostUpdateDto { Title = "Updated Title", Content = "This is the updated post content." };
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
        public async Task UpdatePost_ReturnsBadRequest_WhenAuthorNameIsInvalid()
        {
            var invalidAuthorPost = new BlogPostUpdateDto { Title = "Exploring the Deep Sea: Mysteries Beneath", Content = "Explore the unexplored with this journey into the depths of the sea.", Author = "Ashley123" };
            _controller.ModelState.AddModelError("Author", "Author name can only contain letters and spaces.");

            var result = await _controller.UpdatePost(1, invalidAuthorPost);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task DeletePost_ReturnsOk_WhenDeletionIsSuccessful()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Exploring the Deep Sea: Mysteries Beneath", Content = "Explore the unexplored with this journey into the depths of the sea." };
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
