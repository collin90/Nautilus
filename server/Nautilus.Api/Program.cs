using Nautilus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure IDatabaseClient: use mock when `UseMockDatabase` is true in configuration
if (builder.Configuration.GetValue("UseMockDatabase", false))
{
    builder.Services.AddSingleton<IDatabaseClient, MockClient>();
}
else
{
    builder.Services.AddSingleton<IDatabaseClient, DatabaseClient>();
}

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Nautilus API is running.");
app.MapGet("/api/todos", async (IDatabaseClient dbClient) =>
{
    var todos = await dbClient.QueryAsync<dynamic>("SELECT * FROM Todos");
    return Results.Ok(todos);
});
app.MapGet("/api/todos/{id}", async (int id, IDatabaseClient dbClient) =>
{
    var todo = await dbClient.QuerySingleAsync<dynamic>("SELECT * FROM Todos WHERE Id = @Id", new { Id = id });
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});
app.MapPost("/api/todos", async (dynamic todo, IDatabaseClient dbClient) =>
{
    await dbClient.ExecuteAsync("INSERT INTO Todos (Title, IsCompleted) VALUES (@Title, @IsCompleted)", todo);
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.UseHttpsRedirection();

app.Run();

