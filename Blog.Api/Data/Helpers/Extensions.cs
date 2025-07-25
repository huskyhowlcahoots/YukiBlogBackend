using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.Data.Helpers;

[ExcludeFromCodeCoverage]
public static class Extensions
{
  public static async Task MigrateDb(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
    await dbContext.Database.MigrateAsync();
  }
}
