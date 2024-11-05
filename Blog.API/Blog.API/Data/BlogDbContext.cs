using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Blog.API.Models;
namespace Blog.API.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<BlogPost> Posts { get; set; }
    }

}
