using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.DbEntities.Author;

[ExcludeFromCodeCoverage]
public class AuthorEntity
{
  public int AuthorId { get; set; }

  public required string Name { get; set; } = string.Empty;

  public string Surname { get; set; } = string.Empty;
}
