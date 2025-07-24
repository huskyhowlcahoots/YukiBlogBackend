using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.Dtos.Post;

[ExcludeFromCodeCoverage]
public record class GetPostDto
{
  public int PostId { get; set; }

  public int AuthorId { get; set; }

  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public string Content { get; set; } = string.Empty;
}
