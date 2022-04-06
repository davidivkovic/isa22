using api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<AppDbContext>(o =>
{
    var connection = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION") ??
                     builder.Configuration.GetConnectionString("Postgres");
    o.UseNpgsql(connection);
});

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
await scope.DisposeAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

