using Blog.Api.Data;
using Blog.Api.Dtos.Author;
using Blog.Api.Mappings.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers.Authors;

[ApiController]
[Route("authors")]
[Produces("application/json", "application/xml")]
[Consumes("application/json", "application/xml")]
public class AuthorsController : ControllerBase
{
  private readonly BlogContext _dbContext;

  public AuthorsController(BlogContext dbContext)
  {
    _dbContext = dbContext;
  }

  // GET all /authors
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
  {
    var authors = await _dbContext.Authors
        .Select(a => a.ToDto())
        .AsNoTracking()
        .ToListAsync();

    return Ok(authors);
  }
}
