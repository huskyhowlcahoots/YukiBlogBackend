using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data.Helpers;

public static class Extensions
{
  public static async Task MigrateDb(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
    await dbContext.Database.MigrateAsync();
  }
}
