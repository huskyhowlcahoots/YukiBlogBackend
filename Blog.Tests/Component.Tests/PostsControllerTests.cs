using Blog.Api.Controllers.Posts;
using Blog.Api.Data;
using Blog.Api.DbEntities.Author;
using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace Blog.Tests.Component.Tests;

[TestFixture]
public class PostsControllerTests
{
  private BlogContext _dbContext;
  private PostsController _controller;
  private FusionCache? _fusionCache;
  private readonly AuthorEntity author = new() { Id = 1, Name = "Nicklas", Surname = "Pillay" };

  [SetUp]
  public void SetUp()
  {
    var options = new DbContextOptionsBuilder<BlogContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _dbContext = new BlogContext(options);

    // FusionCache is tricky to set up in tests, so we use a simple in-memory cache to help simulate the cache behavior.
    var fusionCacheOptions = Options.Create(new FusionCacheOptions());
    var memoryCache = new MemoryCache(new MemoryCacheOptions());
    _fusionCache = new FusionCache(fusionCacheOptions, memoryCache);

    _controller = new PostsController(_dbContext, _fusionCache);
  }

  [TearDown]
  public void TearDown()
  {
    _dbContext.Dispose();
    _fusionCache?.Dispose();
  }

  [Test]
  public async Task GetPosts_ReturnsAllPosts_FromDatabase()
  {
    // Arrange
    var post = new PostEntity { Id = 1, AuthorId = 1, Author = author, Title = "DB Post", Description = "Lorem", Content = "Ipsum" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var response = await _controller.GetPosts();

    // Assert
    Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
    var result = (OkObjectResult)response.Result!;
    var posts = (IEnumerable<GetPostDto>)result.Value!;
    Assert.Multiple(() =>
    {
      Assert.That(posts.Count(), Is.EqualTo(1));
      Assert.That(posts.First().Title, Is.EqualTo("DB Post"));
    });
  }

  [Test]
  public async Task GetPosts_ReturnsAllPosts_FromCache_AndIsQuickerThanDatabase()
  {
    // Arrange
    var post = new PostEntity { Id = 20, AuthorId = 1, Author = author, Title = "Cached Post", Description = "Cached Descript", Content = "Cached Content" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act - first call, cache miss, hits DB and populates cache
    var cacheMissTime = System.Diagnostics.Stopwatch.StartNew();
    var response1 = await _controller.GetPosts();
    cacheMissTime.Stop();
    Console.WriteLine($"Cache miss took: {cacheMissTime.ElapsedMilliseconds} ms");

    // Act - second call, should hit cache now (no DB query)
    var cacheHitTime = System.Diagnostics.Stopwatch.StartNew();
    var response2 = await _controller.GetPosts();
    cacheHitTime.Stop();
    Console.WriteLine($"Cache hit took: {cacheHitTime.ElapsedMilliseconds} ms");

    // Assert results from cache hit and second call is faster than first call
    Assert.That(response2.Result, Is.InstanceOf<OkObjectResult>());
    var result = (OkObjectResult)response2.Result!;
    var posts = (IEnumerable<GetPostDto>)result.Value!;
    Assert.Multiple(() =>
    {
      Assert.That(posts.Count(), Is.EqualTo(1));
      Assert.That(posts.First().Title, Is.EqualTo("Cached Post"));
    });
    Assert.That(cacheHitTime.ElapsedMilliseconds, Is.LessThan(cacheMissTime.ElapsedMilliseconds), "Second call should be faster due to cache hit");
  }

  [Test]
  public async Task GetPostById_NoQueryString_ReturnsPost()
  {
    // Arrange
    var post = new PostEntity { Id = 1, AuthorId = 1, Author = author, Title = "SinglePost", Description = "SinglePostDesc", Content = "SinglePostContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var controllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };
    _controller.ControllerContext = controllerContext;
    var response = await _controller.GetPostById(1);

    // Assert
    Assert.That(response, Is.InstanceOf<OkObjectResult>());
    var result = response as OkObjectResult;
    var dto = result!.Value as GetPostDto;
    Assert.That(dto!.Title, Is.EqualTo(post.Title));
  }

  [Test]
  public async Task GetPostById_WithIncorrectQueryString_ReturnsDefaultGetPostDto()
  {
    // Arrange
    var post = new PostEntity
    {
      Id = 1,
      AuthorId = 1,
      Author = author,
      Title = "War and Peace",
      Description = "Historical",
      Content = "A long story"
    };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var httpContext = new DefaultHttpContext();
    httpContext.Request.QueryString = new QueryString("?includeAuthor=banana");
    _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    // Act
    var response = await _controller.GetPostById(1);

    // Assert
    Assert.That(response, Is.InstanceOf<OkObjectResult>());
    var result = response as OkObjectResult;
    var dto = result!.Value as GetPostDto;
    Assert.Multiple(() =>
    {
      Assert.That(dto!.Title, Is.EqualTo(post.Title));
      Assert.That(dto.AuthorId is 1);
      Assert.That(dto is not null);
      Assert.That(dto is GetPostDto); // this is a normal GetPostDto
    });
  }

  [Test]
  public async Task GetPostById_WithIncludeAuthorTrue_ReturnsDtoWithAuthorDetails()
  {
    // Arrange
    var post = new PostEntity
    {
      Id = 9,
      AuthorId = 1,
      Author = author,
      Title = "Tragedy in New York",
      Description = "Tragic Stuff",
      Content = "Big Brother is watching"
    };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var httpContext = new DefaultHttpContext();
    httpContext.Request.QueryString = new QueryString("?includeAuthor=true");
    _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    // Act
    var response = await _controller.GetPostById(9);

    // Assert
    Assert.That(response, Is.InstanceOf<OkObjectResult>());
    var result = response as OkObjectResult;
    var dto = result!.Value as GetPostWithAuthorDetailsDto;

    Assert.Multiple(() =>
    {
      Assert.That(dto, Is.Not.Null);
      Assert.That(dto is GetPostWithAuthorDetailsDto);
      Assert.That(dto!.Title, Is.EqualTo(post.Title));
      Assert.That(dto.AuthorDetails, Is.Not.Null);
      Assert.That(dto.AuthorDetails!.AuthorId, Is.EqualTo(1));
      Assert.That(dto.AuthorDetails!.Name, Is.EqualTo("Nicklas"));
    });
  }


  [Test]
  public async Task AddPost_CreatesNewPost()
  {
    // Arrange
    _dbContext.Authors.Add(author);
    await _dbContext.SaveChangesAsync();

    var newPost = new CreatePostDto
    {
      AuthorId = 1,
      Title = "NewPost",
      Description = "NewDesc",
      Content = "NewContent"
    };

    // Act
    var response = await _controller.AddPost(newPost);

    // Assert
    Assert.That(response.Result, Is.InstanceOf<CreatedAtRouteResult>());
    var createdResult = response.Result as CreatedAtRouteResult;
    Assert.That(createdResult, Is.Not.Null);
    var dto = createdResult.Value as GetPostDto;
    Assert.That(dto, Is.Not.Null);
    Assert.That(dto.Title, Is.EqualTo("NewPost"));
  }

  [Test]
  public async Task UpdatePost_UpdatesExistingPost()
  {
    // Arrange
    var post = new PostEntity { Id = 1, AuthorId = 1, Author = author, Title = "OldTitle", Description = "OldDesc", Content = "OldContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var updatedPost = new UpdatePostDto
    {
      AuthorId = 1,
      Title = "UpdatedTitle",
      Description = "UpdatedDesc",
      Content = "UpdatedContent"
    };

    // Act
    var response = await _controller.UpdatePost(1, updatedPost);

    // Assert
    Assert.That(response, Is.InstanceOf<NoContentResult>());
    var result = await _dbContext.Posts.FindAsync(1);
    Assert.That(result!.Title, Is.EqualTo(updatedPost.Title));
  }

  [Test]
  public async Task DeletePost_RemovesPost()
  {
    // Arrange
    var post = new PostEntity { Id = 7, AuthorId = 7, Author = author, Title = "ToDelete", Description = "ToDeleteDesc", Content = "ToDeleteContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var response = await _controller.DeletePost(7);

    // Assert
    Assert.That(response, Is.InstanceOf<NoContentResult>());
    var result = await _dbContext.Posts.FindAsync(7);
    Assert.That(result, Is.Null);
  }
}
