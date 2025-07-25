using Blog.Api.Data;
using Blog.Api.DbEntities.Author;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Blog.Tests.Integration.Tests;

[TestFixture]
public class BlogContextIntegrationTests
{
  private SqliteConnection _connection;
  private BlogContext _dbContext;

  [SetUp]
  public void SetUp()
  {
    // Use a SQLite in-memory database
    _connection = new SqliteConnection("DataSource=:memory:");
    _connection.Open();

    var options = new DbContextOptionsBuilder<BlogContext>()
        .UseSqlite(_connection)
        .Options;

    _dbContext = new BlogContext(options);
    _dbContext.Database.EnsureCreated(); // just want to make sure the database is created
  }

  [TearDown]
  public void TearDown()
  {
    _dbContext.Dispose();
    _connection.Close();
  }

  [Test]
  public async Task CanInsertAuthor_IntoDatabase()
  {
    // Arrange
    var author = new AuthorEntity { Id = 101, Name = "Insert", Surname = "Test" };

    // Act
    _dbContext.Authors.Add(author);
    await _dbContext.SaveChangesAsync();

    // Assert
    var exists = await _dbContext.Authors.AnyAsync(a => a.Id == 101);
    Assert.That(exists, Is.True);
  }

  [Test]
  public async Task CanReadAuthor_FromDatabase()
  {
    // Arrange
    var seeded = new AuthorEntity { Id = 102, Name = "Read", Surname = "Test" };
    _dbContext.Authors.Add(seeded);
    await _dbContext.SaveChangesAsync();

    // Act
    var fetched = await _dbContext.Authors.FindAsync(102);

    // Assert
    Assert.That(fetched, Is.Not.Null);
    Assert.Multiple(() =>
    {
      Assert.That(fetched!.Name, Is.EqualTo("Read"));
      Assert.That(fetched.Surname, Is.EqualTo("Test"));
    });
  }
}