using Blog.Api.Data;
using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Post;
using Blog.Api.Mappings.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace Blog.Api.Controllers.Posts;

[ApiController]
[Route("posts")]
[Produces("application/json", "application/xml")]
[Consumes("application/json", "application/xml")]
public class PostsController(BlogContext dbContext, IFusionCache fusionCache) : ControllerBase
{
  private const string CacheKey = "all-posts-cache-key";
  private readonly BlogContext _dbContext = dbContext;
  private readonly IFusionCache _fusionCache = fusionCache;

  // GET all via /posts
  [HttpGet]
  public async Task<ActionResult<IEnumerable<GetPostDto>>> GetPosts()
  {
    var posts = await _fusionCache.GetOrSetAsync(
      CacheKey,
      async token =>
      {
        return await _dbContext.Posts
          .Include(post => post.Author)
          .Select(post => post.ToGetPostDto())
          .AsNoTracking()
          .ToListAsync(token);
      },
      TimeSpan.FromMinutes(5)
    );

    return Ok(posts);
  }

  // GET single post by ID via /posts/{id}?includeAuthor=true
  [HttpGet("{id:int}", Name = "GetPostById")]
  public async Task<ActionResult> GetPostById(int id)
  {
    var queryValue = HttpContext.Request.Query["includeAuthor"].ToString();
    bool includeAuthor = queryValue.Equals("true", StringComparison.OrdinalIgnoreCase);

    PostEntity? post;

    if (includeAuthor)
    {
      post = await _dbContext.Posts
          .Include(p => p.Author)
          .FirstOrDefaultAsync(p => p.Id == id);
    }
    else
    {
      post = await _dbContext.Posts.FindAsync(id);
    }

    if (post is null)
      return NotFound();

    if (includeAuthor)
      return Ok(post.ToGetPostWithAuthorDetailsDto());

    return Ok(post.ToGetPostDto());
  }

  // Add a new blog entry via /posts
  [HttpPost]
  public async Task<ActionResult<GetPostDto>> AddPost([FromBody] CreatePostDto newPost)
  {
    PostEntity post = newPost.ToEntity();

    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();
    _fusionCache.Remove(CacheKey);

    var postDto = post.ToGetPostDto();

    return CreatedAtRoute("GetPostById", new { id = post.Id }, postDto);
  }

  // PUT - Update an existing entry via PUT to /posts/{id}
  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatedPost)
  {
    PostEntity? existingPost = await _dbContext.Posts.FindAsync(id);
    if (existingPost is null)
      return NotFound();

    _dbContext.Entry(existingPost)
        .CurrentValues
        .SetValues(updatedPost.ToEntity(id));

    await _dbContext.SaveChangesAsync();
    _fusionCache.Remove(CacheKey);

    return NoContent();
  }

  // DELETE a post via /posts/{id}
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeletePost(int id)
  {
    var post = await _dbContext.Posts.FindAsync(id);
    if (post == null)
      return NotFound();

    _dbContext.Posts.Remove(post);
    await _dbContext.SaveChangesAsync();
    _fusionCache.Remove(CacheKey);

    return NoContent();
  }
}
