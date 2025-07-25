using Blog.Api.DbEntities.Author;
using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Post;
using Blog.Api.Mappings.Post;

namespace Blog.Tests.Unit.Tests;

[TestFixture]
public class PostMappingTests
{
  [Test]
  public void ToEntity_FromCreatePostDto_MapsPropertiesCorrectly()
  {
    // Arrange
    var dto = new CreatePostDto
    {
      AuthorId = 2,
      Title = "Test Title",
      Description = "Test Description",
      Content = "Test Content"
    };

    // Act
    var entity = dto.ToEntity();

    // Assert
    Assert.That(entity.AuthorId, Is.EqualTo(dto.AuthorId));
    Assert.That(entity.Title, Is.EqualTo(dto.Title));
    Assert.That(entity.Description, Is.EqualTo(dto.Description));
    Assert.That(entity.Content, Is.EqualTo(dto.Content));
  }

  [Test]
  public void ToEntity_FromUpdatePostDto_MapsPropertiesCorrectly()
  {
    // Arrange
    var dto = new UpdatePostDto
    {
      AuthorId = 3,
      Title = "Updated Title",
      Description = "Updated Description",
      Content = "Updated Content"
    };
    int id = 42;

    // Act
    var entity = dto.ToEntity(id);

    // Assert
    Assert.That(entity.Id, Is.EqualTo(id));
    Assert.That(entity.AuthorId, Is.EqualTo(dto.AuthorId));
    Assert.That(entity.Title, Is.EqualTo(dto.Title));
    Assert.That(entity.Description, Is.EqualTo(dto.Description));
    Assert.That(entity.Content, Is.EqualTo(dto.Content));
  }

  [Test]
  public void ToGetPostDto_MapsPropertiesCorrectly()
  {
    // Arrange
    var entity = new PostEntity
    {
      Id = 5,
      AuthorId = 7,
      Title = "Post Title",
      Description = "Post Description",
      Content = "Post Content"
    };

    // Act
    var dto = entity.ToGetPostDto();

    // Assert
    Assert.That(dto.PostId, Is.EqualTo(entity.Id));
    Assert.That(dto.AuthorId, Is.EqualTo(entity.AuthorId));
    Assert.That(dto.Title, Is.EqualTo(entity.Title));
    Assert.That(dto.Description, Is.EqualTo(entity.Description));
    Assert.That(dto.Content, Is.EqualTo(entity.Content));
  }

  [Test]
  public void ToGetPostWithAuthorDetailsDto_MapsPropertiesCorrectly()
  {
    // Arrange
    var author = new AuthorEntity
    {
      Id = 10,
      Name = "Jane",
      Surname = "Smith"
    };
    var entity = new PostEntity
    {
      Id = 15,
      AuthorId = author.Id,
      Author = author,
      Title = "Post With Author",
      Description = "Description",
      Content = "Content"
    };

    // Act
    var dto = entity.ToGetPostWithAuthorDetailsDto();

    // Assert
    Assert.That(dto.PostId, Is.EqualTo(entity.Id));
    Assert.That(dto.Title, Is.EqualTo(entity.Title));
    Assert.That(dto.Description, Is.EqualTo(entity.Description));
    Assert.That(dto.Content, Is.EqualTo(entity.Content));

    Assert.That(dto.AuthorDetails, Is.Not.Null);
    Assert.That(dto.AuthorDetails.AuthorId, Is.EqualTo(author.Id));
    Assert.That(dto.AuthorDetails.Name, Is.EqualTo(author.Name));
    Assert.That(dto.AuthorDetails.Surname, Is.EqualTo(author.Surname));
  }
}

