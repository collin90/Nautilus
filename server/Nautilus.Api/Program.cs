using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nautilus.Api.Services;
using Nautilus.Api.Infrastructure.Security;
using Nautilus.Api.Infrastructure.Auth;
using Nautilus.Api.Infrastructure.Profile;
using Nautilus.Api.Infrastructure.Eco;
using Nautilus.Api.Application.Auth;
using Nautilus.Api.Application.Profile;
using Nautilus.Api.Application.Eco;

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
builder.Services.AddSingleton<JwtService>();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register IAuthRepository implementation based on configuration
if (builder.Configuration.GetValue("Backend:Type", "memory") == "memory")
{
    builder.Services.AddSingleton<IAuthRepository, AuthRepositoryMem>();
    builder.Services.AddSingleton<IProfileRepository, ProfileRepositoryMem>();
    builder.Services.AddSingleton<ISpeciesRepository, SpeciesRepositoryMem>();
}
else
{
    builder.Services.AddScoped<IAuthRepository, AuthRepositoryDB>();
    builder.Services.AddScoped<IProfileRepository, ProfileRepositoryDB>();
    builder.Services.AddScoped<ISpeciesRepository, SpeciesRepositoryDB>();
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
app.UseCors();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Base health route
app.MapGet("/", () => "Nautilus server running");

app.MapAuthRoutes();
app.MapProfileRoutes();
app.MapEcoRoutes();


app.Run();