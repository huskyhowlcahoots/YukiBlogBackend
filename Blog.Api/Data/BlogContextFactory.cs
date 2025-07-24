using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Blog.Api.Data;

public class BlogContextFactory : IDesignTimeDbContextFactory<BlogContext>
{
  public BlogContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<BlogContext>();

    var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

    var connectionString = config.GetConnectionString("Blog");
    optionsBuilder.UseSqlite(connectionString);

    return new BlogContext(optionsBuilder.Options);
  }
}
