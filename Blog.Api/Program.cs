using Blog.Api.Data;
using Blog.Api.Data.Helpers;
using System.Diagnostics.CodeAnalysis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace Blog.Api;

[ExcludeFromCodeCoverage]
public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("Blog");
    builder.Services.AddSqlite<BlogContext>(connectionString);

    builder.Services.AddControllers()
      .AddXmlSerializerFormatters();

    builder.Services.AddFusionCache()
      .WithDefaultEntryOptions(new FusionCacheEntryOptions
      {
        Duration = TimeSpan.FromMinutes(5)
      })
      .WithSerializer(new FusionCacheNewtonsoftJsonSerializer());

    var app = builder.Build();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
      await app.MigrateDb();
      Console.WriteLine("Database was migrated successfully.");
    }

    app.Run();
  }
}
