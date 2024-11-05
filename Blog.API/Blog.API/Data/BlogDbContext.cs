using Microsoft.EntityFrameworkCore;
using Blog.API.Models;
namespace Blog.API.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<BlogPost> Posts { get; set; }
    }

}
