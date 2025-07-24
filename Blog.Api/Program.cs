using Blog.Api.Data;
using Blog.Api.Data.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Blog");
builder.Services.AddSqlite<BlogContext>(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();

  await app.MigrateDb();
  Console.WriteLine("Database migrated successfully.");
}

app.MapGet("/", () => "Hi!");

app.Run();
