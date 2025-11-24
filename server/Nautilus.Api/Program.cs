using System.Data;
using Nautilus.Api.Services;
using Nautilus.Api.Infrastructure.Security;
using Nautilus.Api.Infrastructure;
using Nautilus.Api.Application.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure IDatabaseClient: use mock when `UseMockDatabase` is true in configuration
if (builder.Configuration.GetValue("Backend:Type", "memory") == "memory")
{
    builder.Services.AddSingleton<IDatabaseClient, MockClient>();
}
else
{
    builder.Services.AddSingleton<IDatabaseClient, DatabaseClient>();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<PasswordHasher>();
// Register IAuthRepository implementation based on configuration
if (builder.Configuration.GetValue("Backend:Type", "memory") == "memory")
{
    builder.Services.AddSingleton<IAuthRepository, AuthRepositoryMem>();
}
else
{
    builder.Services.AddScoped<IAuthRepository, AuthRepositoryDB>();
}
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Base health route
app.MapGet("/", () => "Nautilus server running");

app.MapAuthRoutes();
app.UseCors();


app.Run();