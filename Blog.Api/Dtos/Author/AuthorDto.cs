﻿using System.Diagnostics.CodeAnalysis;

namespace Blog.Api.Dtos.Author;

[ExcludeFromCodeCoverage]
public record class AuthorDto
{
  public int AuthorId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Surname { get; set; } = string.Empty;
}
