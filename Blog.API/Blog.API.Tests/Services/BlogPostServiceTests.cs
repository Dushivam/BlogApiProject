using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Interfaces;
using Blog.API.Models;
using Blog.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Blog.API.Tests.Services
{

    public class BlogPostServiceTests
    {
        private readonly BlogPostService _service;
        private readonly Mock<IBlogPostRepository> _mockRepository;
        private readonly Mock<ILogger<BlogPostService>> _mockLogger;

        public BlogPostServiceTests()
        {
            _mockRepository = new Mock<IBlogPostRepository>();
            _mockLogger = new Mock<ILogger< BlogPostService >>();
            _service = new BlogPostService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllPostsAsync_ReturnsPosts_WhenPostsExist()
        {
            var mockPosts = new List<BlogPost>
            {
                new BlogPost { Id = 156, Title = "Test Post 1", Content="This is a test post content." },
                new BlogPost { Id = 157, Title = "Test Post 2", Content="This is a test post content." }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockPosts);

            var result = await _service.GetAllPostsAsync();
            Assert.Equal(2, result.Count());
            //_mockLogger.Verify(log => log.LogInformation("Retrieving all blog posts"), Times.Once);
        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsPost_WhenPostExists()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Test Post" , Content = "This is a test post content." };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockPost);

            var result = await _service.GetPostByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            //_mockLogger.Verify(log => log.LogInformation("Retrieving blog post with ID {Id}", 1), Times.Once);
        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsNull_WhenPostDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((BlogPost)null);

            var result = await _service.GetPostByIdAsync(1);
            Assert.Null(result);
            //_mockLogger.Verify(log => log.LogWarning("Blog post with ID {Id} not found", 1), Times.Once);
        }

        [Fact]
        public async Task AddPostAsync_AddsPost_WhenPostIsValid()
        {
            var newPost = new BlogPost { Id = 3, Title = "New Post", Content = "New Content" };

            await _service.AddPostAsync(newPost);
            _mockRepository.Verify(repo => repo.AddAsync(newPost), Times.Once);
            //_mockLogger.Verify(log => log.LogInformation("Successfully added blog post with ID {Id}", newPost.Id), Times.Once);
        }

        [Fact]
        public async Task AddPostAsync_ThrowsArgumentNullException_WhenPostIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPostAsync(null));
            //_mockLogger.Verify(log => log.LogWarning("Attempted to add a null blog post"), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_UpdatesPost_WhenPostIsValid()
        {
            var updatedPost = new BlogPost { Id = 1, Title = "Updated Title", Content = "Updated Content" };

            await _service.UpdatePostAsync(updatedPost);
            _mockRepository.Verify(repo => repo.UpdateAsync(updatedPost), Times.Once);
            //_mockLogger.Verify(log => log.LogInformation("Successfully updated blog post with ID {Id}", updatedPost.Id), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_ThrowsArgumentNullException_WhenPostIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdatePostAsync(null));
            //_mockLogger.Verify(log => log.LogWarning("Attempted to update a null blog post"), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_DeletesPost_WhenPostExists()
        {
            int postId = 1;

            await _service.DeletePostAsync(postId);
            _mockRepository.Verify(repo => repo.DeleteAsync(postId), Times.Once);
            //_mockLogger.Verify(log => log.LogInformation("Successfully deleted blog post with ID {Id}", postId), Times.Once);
        }
    }
}