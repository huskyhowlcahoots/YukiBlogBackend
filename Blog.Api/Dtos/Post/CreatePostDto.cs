using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.Dtos.Post;

[ExcludeFromCodeCoverage]
public record class CreatePostDto
{
  public int PostId { get; set; }

  [Required]
  public int AuthorId { get; set; }

  [Required]
  [StringLength(100)]
  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  [Required]
  [StringLength(5000)]
  public string Content { get; set; } = string.Empty;
}