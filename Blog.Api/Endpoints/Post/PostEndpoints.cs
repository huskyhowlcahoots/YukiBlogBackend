using Blog.Api.Data;
using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Post;
using Blog.Api.Mappings.Post;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Endpoints.Post;

public static class PostEndpoints
{
  // Extension method for WebApplication - regarding Mapping, to keep Program.cs cleaner
  public static WebApplication MapPostEndpoints(this WebApplication app)
  {
    GetPosts(app);
    GetPostById(app);
    AddPost(app);
    UpdatePost(app);
    DeletePost(app);

    return app;
  }

  private static void GetPosts(WebApplication app)
  {
    // GET all via /posts
    app.MapGet("/posts", async (BlogContext dbContext) =>
    {
      var posts = await dbContext.Posts
          .Include(post => post.Author)
          .Select(post => post.ToGetPostDto())
          .AsNoTracking()
          .ToListAsync();

      return Results.Ok(posts);
    });
  }

  private static void GetPostById(WebApplication app)
  {
    // GET single post by ID via /posts/{id}
    app.MapGet("/posts/{id:int}", async (int id, HttpRequest request, BlogContext dbContext) =>
    {
      var queryValue = request.Query["includeAuthor"].ToString();
      bool includeAuthor = queryValue.Equals("true", StringComparison.OrdinalIgnoreCase);

      PostEntity? post = null;

      if (includeAuthor)
      {
        post = await dbContext.Posts
            .Include(p => p.Author) // include author data in the result
            .FirstOrDefaultAsync(p => p.Id == id); // for this post id
      }
      else
      {
        post = await dbContext.Posts.FindAsync(id); // Find is efficient for primary key lookups
      }

      if (post is null)
        return Results.NotFound();

      if (includeAuthor)
      {
        return Results.Ok(post.ToGetPostWithAuthorDetailsDto());
      }

      return Results.Ok(post.ToGetPostDto());
    })
    .WithName("GetPostById");
  }

  private static void AddPost(WebApplication app)
  {
    // Add a new blog entry via /posts
    app.MapPost("/posts", async (CreatePostDto newPost, BlogContext dbContext) =>
    {
      PostEntity post = newPost.ToEntity();

      dbContext.Posts.Add(post);
      await dbContext.SaveChangesAsync();

      var postDto = post.ToGetPostDto();

      // After adding the post to the list,
      // Return the resource location in the response
      // using CreatedAtRoute and the GetpostById route from above
      return Results.CreatedAtRoute("GetPostById", new { id = post.Id }, postDto);
    })
      .WithParameterValidation();
  }

  private static void UpdatePost(WebApplication app)
  {
    // Update an existing entry via PUT to /posts/{id}
    app.MapPut("/posts/{id:int}", async (int id, UpdatePostDto updatedPost, BlogContext dbContext) =>
    {
      PostEntity? existingPost = await dbContext.Posts.FindAsync(id);
      if (existingPost is null)
        return Results.NotFound();

      dbContext.Entry(existingPost)
          .CurrentValues
          .SetValues(updatedPost.ToEntity(id));

      await dbContext.SaveChangesAsync();

      return Results.NoContent();
    });
  }

  private static void DeletePost(WebApplication app)
  {
    // DELETE a post via /posts/{id}
    app.MapDelete("/posts/{id:int}", async (int id, BlogContext dbContext) =>
    {
      await dbContext.Posts
          .Where(post => post.Id == id)
          .ExecuteDeleteAsync();

      return Results.NoContent();
    });
  }
}
