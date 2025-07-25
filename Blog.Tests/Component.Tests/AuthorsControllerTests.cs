using Blog.Api.Controllers.Authors;
using Blog.Api.Data;
using Blog.Api.DbEntities.Author;
using Blog.Api.Dtos.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Tests.Component.Tests;

[TestFixture]
public class AuthorsControllerTests
{
  private AuthorsController _controller;
  private BlogContext _dbContext;

  [SetUp]
  public void SetUp()
  {
    // Arrange - new in-memory database for each test
    var options = new DbContextOptionsBuilder<BlogContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _dbContext = new BlogContext(options);

    // Seed the in-memory database
    _dbContext.Authors.AddRange(
      new AuthorEntity { Id = 1, Name = "John", Surname = "Doe" },
      new AuthorEntity { Id = 2, Name = "Jane", Surname = "Smith" }
    );
    _dbContext.SaveChanges();

    _controller = new AuthorsController(_dbContext);
  }

  [TearDown]
  public void TearDown()
  {
    _dbContext.Dispose();
  }

  [Test]
  public async Task GetAuthors_ReturnsSuccessful_WithExpectedAuthors()
  {
    // Act
    var result = await _controller.GetAuthors();

    // Assert
    var okResult = result.Result as OkObjectResult;
    Assert.That(okResult, Is.Not.Null);

    var authors = okResult!.Value as IEnumerable<AuthorDto>;
    Assert.That(authors, Is.Not.Null);
    Assert.That(authors!.Count(), Is.EqualTo(2));

    var list = authors.ToList();

    Assert.Multiple(() =>
    {
      Assert.That(list[0].AuthorId, Is.EqualTo(1));
      Assert.That(list[0].Name, Is.EqualTo("John"));
      Assert.That(list[0].Surname, Is.EqualTo("Doe"));

      Assert.That(list[1].AuthorId, Is.EqualTo(2));
      Assert.That(list[1].Name, Is.EqualTo("Jane"));
      Assert.That(list[1].Surname, Is.EqualTo("Smith"));
    });
  }
}
