using Blog.Api.Data;
using Blog.Api.Data.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();

  await app.MigrateDb();
  Console.WriteLine("Database migrated successfully.");
}

var connectionString = builder.Configuration.GetConnectionString("Blog");
builder.Services.AddSqlite<BlogContext>(connectionString);

app.MapGet("/", () => "Hi!");

app.Run();
