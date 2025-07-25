using Blog.Api.Data;
using Blog.Api.Data.Helpers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Blog");
builder.Services.AddSqlite<BlogContext>(connectionString);

builder.Services.AddControllers()
  .AddXmlSerializerFormatters();

var app = builder.Build();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
  await app.MigrateDb();
  Console.WriteLine("Database was migrated successfully.");
}

app.Run();
