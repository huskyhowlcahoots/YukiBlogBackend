using Blog.Api.DbEntities.Post;
using Blog.Api.Dtos.Author;
using Blog.Api.Dtos.Post;

namespace Blog.Api.Mappings.Post;

public static class PostMapping
{
  public static PostEntity ToEntity(this CreatePostDto post)
  {
    return new()
    {
      AuthorId = post.AuthorId,
      Title = post.Title,
      Description = post.Description,
      Content = post.Content,
    };
  }

  public static PostEntity ToEntity(this UpdatePostDto post, int id)
  {
    return new()
    {
      Id = id,
      AuthorId = post.AuthorId,
      Title = post.Title,
      Description = post.Description,
      Content = post.Content,
    };
  }

  public static GetPostDto ToGetPostDto(this PostEntity post)
  {
    return new()
    {
      PostId = post.Id,
      AuthorId = post.AuthorId,
      Title = post.Title,
      Description = post.Description,
      Content = post.Content
    };
  }

  public static GetPostWithAuthorDetailsDto ToGetPostWithAuthorDetailsDto(this PostEntity post)
  {
    AuthorDto? authorDetails = new()
    {
      AuthorId = post.Author!.Id,
      Name = post.Author.Name,
      Surname = post.Author.Surname
    };

    return new()
    {
      PostId = post.Id,
      AuthorDetails = authorDetails,
      Title = post.Title,
      Description = post.Description,
      Content = post.Content,
    };
  }
}
