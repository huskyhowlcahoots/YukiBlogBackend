using Blog.Api.Controllers.Posts;
using Blog.Api.Data;
using Blog.Api.DbEntities.Author;
using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Tests.Component.Tests;

[TestFixture]
public class PostsControllerTests
{
  private BlogContext _dbContext;
  private PostsController _controller;

  [SetUp]
  public void SetUp()
  {
    var options = new DbContextOptionsBuilder<BlogContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    _dbContext = new BlogContext(options);
    _controller = new PostsController(_dbContext);
  }

  [TearDown]
  public void TearDown()
  {
    _dbContext.Dispose();
  }

  [Test]
  public async Task GetPosts_ReturnsAllPosts()
  {
    // Arrange
    var author = new AuthorEntity { Id = 1, Name = "John", Surname = "Doe" };
    var post = new PostEntity { Id = 1, AuthorId = 1, Author = author, Title = "Test1", Description = "Desc", Content = "Content" };

    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _controller.GetPosts();

    // Assert
    Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    var okResult = result.Result as OkObjectResult;
    var posts = okResult!.Value as IEnumerable<GetPostDto>;
    Assert.Multiple(() =>
    {
      Assert.That(posts!.Count(), Is.EqualTo(1));
      Assert.That(posts!.First().Title, Is.EqualTo("Test1"));
    });
  }


  [Test]
  public async Task GetPostById_NoQueryString_ReturnsPost()
  {
    // Arrange
    var author = new AuthorEntity { Id = 3, Name = "Alice", Surname = "Brown" };
    var post = new PostEntity { Id = 3, AuthorId = 3, Author = author, Title = "SinglePost", Description = "SinglePostDesc", Content = "SinglePostContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var controllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };
    _controller.ControllerContext = controllerContext;
    var result = await _controller.GetPostById(3);

    // Assert
    Assert.That(result, Is.InstanceOf<OkObjectResult>());
    var okResult = result as OkObjectResult;
    var dto = okResult!.Value as GetPostDto;
    Assert.That(dto!.Title, Is.EqualTo("SinglePost"));
  }

  [Test]
  public async Task GetPostById_WithRandomQueryString_ReturnsDefaultGetPostDto()
  {
    // Arrange
    var author = new AuthorEntity { Id = 1, Name = "Leo", Surname = "Tolstoy" };
    var post = new PostEntity
    {
      Id = 1,
      AuthorId = 1,
      Author = author,
      Title = "War and Peace",
      Description = "Historical Novel",
      Content = "A long story"
    };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var context = new DefaultHttpContext();
    context.Request.QueryString = new QueryString("?includeAuthor=banana");
    _controller.ControllerContext = new ControllerContext { HttpContext = context };

    // Act
    var result = await _controller.GetPostById(1);

    // Assert
    Assert.That(result, Is.InstanceOf<OkObjectResult>());
    var okResult = result as OkObjectResult;
    var dto = okResult!.Value as GetPostDto;
    Assert.Multiple(() =>
    {
      Assert.That(dto!.Title, Is.EqualTo(post.Title));
      Assert.That(dto.AuthorId is 1);
      Assert.That(dto is not null);
      Assert.That(dto is GetPostDto);
    });
  }

  [Test]
  public async Task GetPostById_WithIncludeAuthorTrue_ReturnsDtoWithAuthorDetails()
  {
    // Arrange
    var author = new AuthorEntity { Id = 2, Name = "George", Surname = "Orwell" };
    var post = new PostEntity
    {
      Id = 2,
      AuthorId = 2,
      Author = author,
      Title = "1984",
      Description = "Dystopian Novel",
      Content = "Big Brother is watching"
    };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var context = new DefaultHttpContext();
    context.Request.QueryString = new QueryString("?includeAuthor=true");
    _controller.ControllerContext = new ControllerContext { HttpContext = context };

    // Act
    var result = await _controller.GetPostById(2);

    // Assert
    Assert.That(result, Is.InstanceOf<OkObjectResult>());
    var okResult = result as OkObjectResult;
    var dto = okResult!.Value as GetPostWithAuthorDetailsDto;

    Assert.Multiple(() =>
    {
      Assert.That(dto, Is.Not.Null);
      Assert.That(dto is GetPostWithAuthorDetailsDto);
      Assert.That(dto!.Title, Is.EqualTo("1984"));
      Assert.That(dto.AuthorDetails, Is.Not.Null);
      Assert.That(dto.AuthorDetails!.AuthorId, Is.EqualTo(2));
      Assert.That(dto.AuthorDetails!.Name, Is.EqualTo("George"));
    });
  }


  [Test]
  public async Task AddPost_CreatesNewPost()
  {
    // Arrange
    var author = new AuthorEntity { Id = 5, Name = "Carl", Surname = "Green" };
    _dbContext.Authors.Add(author);
    await _dbContext.SaveChangesAsync();

    var newPost = new CreatePostDto
    {
      AuthorId = 5,
      Title = "NewPost",
      Description = "NewDesc",
      Content = "NewContent"
    };

    // Act
    var result = await _controller.AddPost(newPost);

    // Assert
    Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
    var createdResult = result.Result as CreatedAtRouteResult;
    Assert.That(createdResult, Is.Not.Null);
    var dto = createdResult.Value as GetPostDto;
    Assert.That(dto, Is.Not.Null);
    Assert.That(dto.Title, Is.EqualTo("NewPost"));
  }

  [Test]
  public async Task UpdatePost_UpdatesExistingPost()
  {
    // Arrange
    var author = new AuthorEntity { Id = 6, Name = "Dana", Surname = "Blue" };
    var post = new PostEntity { Id = 6, AuthorId = 6, Author = author, Title = "OldTitle", Description = "OldDesc", Content = "OldContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    var updatedPost = new UpdatePostDto
    {
      AuthorId = 6,
      Title = "UpdatedTitle",
      Description = "UpdatedDesc",
      Content = "UpdatedContent"
    };

    // Act
    var result = await _controller.UpdatePost(6, updatedPost);

    // Assert
    Assert.That(result, Is.InstanceOf<NoContentResult>());
    var updated = await _dbContext.Posts.FindAsync(6);
    Assert.That(updated!.Title, Is.EqualTo("UpdatedTitle"));
  }

  [Test]
  public async Task DeletePost_RemovesPost()
  {
    // Arrange
    var author = new AuthorEntity { Id = 7, Name = "Eve", Surname = "Black" };
    var post = new PostEntity { Id = 7, AuthorId = 7, Author = author, Title = "ToDelete", Description = "ToDeleteDesc", Content = "ToDeleteContent" };
    _dbContext.Authors.Add(author);
    _dbContext.Posts.Add(post);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _controller.DeletePost(7);

    // Assert
    Assert.That(result, Is.InstanceOf<NoContentResult>());
    var deleted = await _dbContext.Posts.FindAsync(7);
    Assert.That(deleted, Is.Null);
  }
}
