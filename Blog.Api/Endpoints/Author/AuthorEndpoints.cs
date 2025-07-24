using Blog.Api.Data;
using Blog.Api.Mappings.Author;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Endpoints.Author;

public static class AuthorEndpoints
{
  public static WebApplication MapAuthorEndpoints(this WebApplication app)
  {
    GetAuthors(app);
    return app;
  }

  public static void GetAuthors(WebApplication app)
  {
    app.MapGet("/authors", async (BlogContext dbContext) =>
    {
      var genres = await dbContext.Authors
          .Select(a => a.ToDto())
          .AsNoTracking()
          .ToListAsync();

      return Results.Ok(genres);
    });
  }
}
