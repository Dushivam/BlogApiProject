using Blog.API.Data;
using Blog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Blog.API.Tests.Repositories
{
    public class BlogPostRepositoryTests
    {
        private readonly BlogDbContext _context;
        private readonly BlogPostRepository _repository;
        private readonly Mock<ILogger<BlogPostRepository>> _mockLogger;

        public BlogPostRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BlogDbContext(options);
            _mockLogger = new Mock<ILogger<BlogPostRepository>>();
            _repository = new BlogPostRepository(_context, _mockLogger.Object);

            _context.Posts.Add(new BlogPost { Id = 699, Title = "Test Post 1", Content = "Content 1", Author = "Author 1", PublishedDate = DateTime.Now });
            _context.Posts.Add(new BlogPost { Id = 700, Title = "Test Post 2", Content = "Content 2", Author = "Author 2", PublishedDate = DateTime.Now });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPosts()
        {
            var posts = await _repository.GetAllAsync();

            Assert.Equal(2, posts.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPost_WhenPostExists()
        {
            var post = await _repository.GetByIdAsync(699);

            Assert.NotNull(post);
            Assert.Equal(699, post.Id);
            Assert.Equal("Test Post 1", post.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenPostDoesNotExist()
        {
            var post = await _repository.GetByIdAsync(99);
            Assert.Null(post);
        }

        [Fact]
        public async Task AddAsync_AddsNewPost()
        {
            var postList = await _repository.GetAllAsync();
            int postCount = postList.Count();

            var newPost = new BlogPost { Id = 993, Title = "New Post", Content = "New Content", Author = "New Author", PublishedDate = DateTime.Now };
            await _repository.AddAsync(newPost);
            var post = await _context.Posts.FindAsync(993);

            Assert.NotNull(post);
            Assert.Equal(993, post.Id);
            Assert.Equal("New Post", post.Title);
            Assert.Equal(postCount + 1, _context.Posts.Count());

        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingPost()
        {
            var updatedPost = new BlogPost { Id = 699, Title = "Updated Title", Content = "Updated Content", Author = "Updated Author", PublishedDate = DateTime.Now };
            await _repository.UpdateAsync(updatedPost);
            var post = await _context.Posts.FindAsync(699);

            Assert.NotNull(post);
            Assert.Equal("Updated Title", post.Title);
            Assert.Equal("Updated Content", post.Content);
        }

        [Fact]
        public async Task DeleteAsync_RemovesPost_WhenPostExists()
        {
            var postList = await _repository.GetAllAsync();
            int postCount = postList.Count();

            await _repository.DeleteAsync(700);
            var post = await _context.Posts.FindAsync(700);

            Assert.Null(post);
            Assert.Equal(postCount - 1, _context.Posts.Count());
        }

        [Fact]
        public async Task DeleteAsync_DoesNothing_WhenPostDoesNotExist()
        {
            await _repository.DeleteAsync(99);
            var postsCount = _context.Posts.Count();

            Assert.Equal(2, postsCount); 
        }
    }
}
