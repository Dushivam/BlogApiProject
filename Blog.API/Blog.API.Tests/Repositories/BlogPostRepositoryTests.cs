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

            _context.Posts.Add(new BlogPost { Id = 699, Title = "The History of the Roman Empire", Content = "An exploration into the rise and fall of one of history’s greatest empires.", Author = "Gareth", PublishedDate = DateTime.Now.AddDays(-10) });
            _context.Posts.Add(new BlogPost { Id = 700, Title = "Mastering Public Speaking", Content = "Practical tips and exercises for overcoming fear and speaking confidently.", Author = "Southgate", PublishedDate = DateTime.Now.AddDays(-10) });
            _context.SaveChanges();
        }

        [Fact]
        public async Task QueryAllPostsAsync_ReturnsAllPosts()
        {
            var posts = await _repository.QueryAllPostsAsync();
            Assert.Equal(2, posts.Count());
        }

        [Fact]
        public async Task QueryAllPostsAsync_FiltersByTitle_ReturnsMatchingPosts()
        {
            await _context.Posts.AddRangeAsync(new List<BlogPost>
            {
                new BlogPost { Id = 1, Title = "Journey to the Center of the Earth", Content = "Exploring Earth's core.", Author = "Jules Verne", PublishedDate = DateTime.Now.AddDays(-10) },
                new BlogPost { Id = 2, Title = "Mastering the Art of French Cooking", Content = "Cooking techniques.", Author = "Julia Child", PublishedDate = DateTime.Now.AddDays(-5) }
            });
            await _context.SaveChangesAsync();

            var posts = await _repository.QueryAllPostsAsync(title: "Journey");

            Assert.Single(posts);
            Assert.Equal("Journey to the Center of the Earth", posts.First().Title);
        }


        [Fact]
        public async Task QueryAllPostsAsync_FiltersByDateRange_ReturnsMatchingPosts()
        {
            var today = DateTime.Now;
            await _context.Posts.AddRangeAsync(new List<BlogPost>
            {
                new BlogPost { Id = 1, Title = "Journey to the Center of the Earth", Content = "Exploring Earth's core.", Author = "Jules Verne", PublishedDate = today.AddDays(-10) },
                new BlogPost { Id = 2, Title = "Mastering the Art of French Cooking", Content = "Cooking techniques.", Author = "Julia Child", PublishedDate = today.AddDays(-5) },
                new BlogPost { Id = 3, Title = "The Future of Artificial Intelligence", Content = "AI advancements.", Author = "John McCarthy", PublishedDate = today }
            });
            await _context.SaveChangesAsync();

            var startDate = today.AddDays(-7);
            var endDate = today;
            var posts = await _repository.QueryAllPostsAsync(startDate: startDate, endDate: endDate);

            Assert.Equal(2, posts.Count());
            Assert.All(posts, post => Assert.True(post.PublishedDate >= startDate && post.PublishedDate <= endDate));
        }

        [Fact]
        public async Task GetPostRecordByIdAsync_ReturnsPost_WhenPostExists()
        {
            var post = await _repository.GetPostRecordByIdAsync(699);

            Assert.NotNull(post);
            Assert.Equal(699, post.Id);
            Assert.Equal("The History of the Roman Empire", post.Title);
        }

        [Fact]
        public async Task GetPostRecordByIdAsync_ReturnsNull_WhenPostDoesNotExist()
        {
            var post = await _repository.GetPostRecordByIdAsync(99);
            Assert.Null(post);
        }

        [Fact]
        public async Task AddPostRecordAsync_AddsNewPost()
        {
            var postList = await _repository.QueryAllPostsAsync();
            int postCount = postList.Count();

            var newPost = new BlogPost { Id = 993, Title = "Top 10 Travel Destinations for Adventure Lovers", Content = "A guide to the most exciting travel destinations for thrill-seekers", Author = "Janette", PublishedDate = DateTime.Now };
            await _repository.AddPostRecordAsync(newPost);
            var post = await _context.Posts.FindAsync(993);

            Assert.NotNull(post);
            Assert.Equal(993, post.Id);
            Assert.Equal("Top 10 Travel Destinations for Adventure Lovers", post.Title);
            Assert.Equal(postCount + 1, _context.Posts.Count());

        }

        [Fact]
        public async Task UpdatePostRecordAsync_UpdatesExistingPost()
        {
            var updatedPost = new BlogPost { Id = 699, Title = "Updated Title", Content = "Updated Content", Author = "Tuchel", PublishedDate = DateTime.Now };
            await _repository.UpdatePostRecordAsync(updatedPost);
            var post = await _context.Posts.FindAsync(699);

            Assert.NotNull(post);
            Assert.Equal("Updated Title", post.Title);
            Assert.Equal("Updated Content", post.Content);
            Assert.Equal("Tuchel", post.Author);
        }

        [Fact]
        public async Task DeletePostRecordAsync_RemovesPost_WhenPostExists()
        {
            var postList = await _repository.QueryAllPostsAsync();
            int postCount = postList.Count();

            await _repository.DeletePostRecordAsync(700);
            var post = await _context.Posts.FindAsync(700);

            Assert.Null(post);
            Assert.Equal(postCount - 1, _context.Posts.Count());
        }

        [Fact]
        public async Task DeletePostRecordAsync_DoesNothing_WhenPostDoesNotExist()
        {
            await _repository.DeletePostRecordAsync(99);
            var postsCount = _context.Posts.Count();

            Assert.Equal(2, postsCount); 
        }
    }
}
