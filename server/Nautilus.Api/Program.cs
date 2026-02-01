using Nautilus.Api.Configuration;
using Nautilus.Api.Application.Auth;
using Nautilus.Api.Application.Profile;
using Nautilus.Api.Application.Eco;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI and Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configure external API clients
builder.Services.AddExternalClients();

// Configure JWT authentication and authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure backend implementations (memory or database)
builder.Services.AddBackends(builder.Configuration);

// Configure application services
builder.Services.AddApplicationServices();

// Configure CORS
builder.Services.AddCorsPolicy();

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