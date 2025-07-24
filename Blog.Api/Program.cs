using Blog.Api.Data;
using Blog.Api.Data.Helpers;
using Blog.Api.Endpoints.Author;
using Blog.Api.Endpoints.Post;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Blog");
builder.Services.AddSqlite<BlogContext>(connectionString);

var app = builder.Build();

app.MapAuthorEndpoints();
app.MapPostEndpoints();

if (app.Environment.IsDevelopment())
{
  await app.MigrateDb();
  Console.WriteLine("Database was migrated successfully.");
}

app.Run();
