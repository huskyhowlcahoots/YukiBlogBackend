using Blog.Api.DbEntities.Author;
using Blog.Api.Dtos.Author;

namespace Blog.Api.Mappings.Author;

public static class AuthorMapping
{
  public static AuthorDto ToDto(this AuthorEntity author)
  {
    return new AuthorDto
    {
      AuthorId = author.Id,
      Name = author.Name,
      Surname = author.Surname,
    };
  }
}
