using Blog.Api.DbEntities.Author;
using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.DbEntities.Post;

[ExcludeFromCodeCoverage]
public class PostEntity
{
  public int Id { get; set; }

  public int AuthorId { get; set; }

  public AuthorEntity? Author { get; set; }

  public required string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public required string Content { get; set; } = string.Empty;
}
