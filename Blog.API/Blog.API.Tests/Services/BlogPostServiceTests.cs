using System;
using System.Collections.Generic;
using System.Linq;
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
            _mockLogger = new Mock<ILogger<BlogPostService>>();
            _service = new BlogPostService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllPostsAsync_ReturnsPosts_WhenPostsExist()
        {
            var mockPosts = new List<BlogPost>
            {
                new BlogPost { Id = 156, Title = "Understanding Climate Change and Its Impact", Content="A look at how climate change affects our environment and daily lives." },
                new BlogPost { Id = 157, Title = "Unleashing the Potential of Remote Work", Content="How to stay productive and connected while working remotely." }
            };
            _mockRepository.Setup(repo => repo.QueryAllPostsAsync(null, null, null, null)).ReturnsAsync(mockPosts);
            var result = await _service.GetAllPostsAsync(null, null, null, null);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPostsAsync_CallsRepositoryWithTitleFilter()
        {
            var title = "Climate Change";
            var mockPosts = new List<BlogPost>
            {
                new BlogPost { Id = 156, Title = "Understanding Climate Change and Its Impact", Content="A look at how climate change affects our environment and daily lives." }
            };
            _mockRepository.Setup(repo => repo.QueryAllPostsAsync(title, null, null, null)).ReturnsAsync(mockPosts);

            var result = await _service.GetAllPostsAsync(title);
            Assert.Single(result);
            _mockRepository.Verify(repo => repo.QueryAllPostsAsync(title, null, null, null), Times.Once);
        }

        [Fact]
        public async Task GetAllPostsPostsAsync_CallsRepositoryWithAuthorFilter()
        {
            var author = "John Doe";
            var mockPosts = new List<BlogPost>
            {
                new BlogPost { Id = 158, Title = "The Future of Work", Content="Exploring changes in the workplace.", Author = "John Doe" }
            };
            _mockRepository.Setup(repo => repo.QueryAllPostsAsync(null, author, null, null)).ReturnsAsync(mockPosts);

            var result = await _service.GetAllPostsAsync(author: author);
            Assert.Single(result);
            _mockRepository.Verify(repo => repo.QueryAllPostsAsync(null, author, null, null), Times.Once);
        }


        [Fact]
        public async Task GetPostByIdAsync_ReturnsPost_WhenPostExists()
        {
            var mockPost = new BlogPost { Id = 1, Title = "Understanding Climate Change and Its Impact", Content = "A look at how climate change affects our environment and daily lives." };
            _mockRepository.Setup(repo => repo.GetPostRecordByIdAsync(1)).ReturnsAsync(mockPost);

            var result = await _service.GetPostByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);

        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsNull_WhenPostDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.GetPostRecordByIdAsync(1)).ReturnsAsync((BlogPost)null);

            var result = await _service.GetPostByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddPostAsync_AddsPost_WhenPostIsValid()
        {
            var newPost = new BlogPost { Id = 3, Title = "Unleashing the Potential of Remote Work", Content = "How to stay productive and connected while working remotely." };

            await _service.AddPostAsync(newPost);

            _mockRepository.Verify(repo => repo.AddPostRecordAsync(newPost), Times.Once);
        }

        [Fact]
        public async Task AddPostAsync_ThrowsArgumentNullException_WhenPostIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPostAsync(null));
        }

        [Fact]
        public async Task UpdatePostAsync_UpdatesPost_WhenPostIsValid()
        {
            var updatedPost = new BlogPost { Id = 1, Title = "Updated Title", Content = "Updated Content" };

            await _service.UpdatePostAsync(updatedPost);

            _mockRepository.Verify(repo => repo.UpdatePostRecordAsync(updatedPost), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_ThrowsArgumentNullException_WhenPostIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdatePostAsync(null));
        }

        [Fact]
        public async Task DeletePostAsync_DeletesPost_WhenPostExists()
        {
            int postId = 156;

            await _service.DeletePostAsync(postId);

            _mockRepository.Verify(repo => repo.DeletePostRecordAsync(postId), Times.Once);
        }
    }
}
