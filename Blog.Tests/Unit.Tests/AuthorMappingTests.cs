using Blog.Api.DbEntities.Author;
using Blog.Api.Mappings.Author;

namespace Blog.Tests.Unit.Tests;

[TestFixture]
public class AuthorMappingTests
{
  [Test]
  public void ToDto_MapsPropertiesCorrectly()
  {
    // Arrange
    var authorEntity = new AuthorEntity
    {
      Id = 1,
      Name = "R.L",
      Surname = "Stine"
    };

    // Act
    var dto = authorEntity.ToDto();

    // Assert
    Assert.That(dto.AuthorId, Is.EqualTo(authorEntity.Id));
    Assert.That(dto.Name, Is.EqualTo(authorEntity.Name));
    Assert.That(dto.Surname, Is.EqualTo(authorEntity.Surname));
  }
}
