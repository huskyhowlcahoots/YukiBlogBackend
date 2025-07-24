using Blog.Api.DbEntities.Author;
using Blog.Api.DbEntities.Post;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
  public DbSet<PostEntity> Posts => Set<PostEntity>();

  public DbSet<AuthorEntity> Authors => Set<AuthorEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Seeding data for Authors
    modelBuilder.Entity<AuthorEntity>().HasData(
      new AuthorEntity { Id = 1, Name = "William", Surname = "Shakespeare" },
      new AuthorEntity { Id = 2, Name = "J.K", Surname = "Rowling" },
      new AuthorEntity { Id = 3, Name = "George R.R", Surname = "Martin" },
      new AuthorEntity { Id = 4, Name = "Charles", Surname = "Dickens" },
      new AuthorEntity { Id = 5, Name = "Stephen", Surname = "King" }
    );
  }
}
