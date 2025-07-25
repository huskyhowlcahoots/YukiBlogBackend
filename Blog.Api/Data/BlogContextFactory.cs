using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.Data;

// Excluding this class from code coverage as it is used only for design-time database context creation upfront
[ExcludeFromCodeCoverage]
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
