using Blog.Api.DbEntities.Author;
using Blog.Api.DbEntities.Post;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
  public DbSet<PostEntity> Posts => Set<PostEntity>();

  public DbSet<AuthorEntity> Authors => Set<AuthorEntity>();
}
