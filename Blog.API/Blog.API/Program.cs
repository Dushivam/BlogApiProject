using Blog.API.Data;
using Blog.API.Interfaces;
using Blog.API.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddOpenApiDocument(document => {
    document.Title = "Blog APIs";
    document.Description = "A RESTful API for managing blog posts.";
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();
app.UseOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    foreach (var address in addresses)
    {
        Log.Information("Application is running on: {Address}", address);
    }
});

Log.Information("Blog API application is running.");
app.Run();