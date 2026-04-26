using EventTicketingAiPlatform.Api.Middleware;
using EventTicketingAiPlatform.Application.DependencyInjection;
using EventTicketingAiPlatform.Infrastructure.DependencyInjection;
using EventTicketingAiPlatform.Infrastructure.Persistence;
using EventTicketingAiPlatform.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
//builder.Services.AddInMemoryInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpsCenter", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

builder.Services.AddPostgresInfrastructure(
    builder.Configuration,
    connectionString);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EventTicketingDbContext>();

    await db.Database.MigrateAsync();
    await PostgreSqlSeed.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("OpsCenter");
app.MapControllers();

app.Run();

public partial class Program
{
}