using ClubDeBarrio.Api.Assistant;
using ClubDeBarrio.Api.Roster;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var databasePath = Path.GetFullPath(
    builder.Configuration["Roster:DatabasePath"] ?? "data/club.db",
    builder.Environment.ContentRootPath);
Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
var connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();

builder.Services.AddDbContext<RosterDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<RosterService>();
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<RosterExceptionHandler>();
builder.Services.AddRosterAssistant();

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    await RosterSeed.InitializeAsync(scope.ServiceProvider.GetRequiredService<RosterDbContext>());
}

app.UseExceptionHandler();
app.MapRosterEndpoints();
app.MapRosterAssistant();
await app.RunAsync();
