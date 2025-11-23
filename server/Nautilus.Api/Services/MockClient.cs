namespace Nautilus.Api.Services;

public class MockClient : IDatabaseClient
{

    private readonly List<dynamic> _todos = new List<dynamic>
    {
        new { Id = 1, Title = "Buy groceries", IsCompleted = false },
        new { Id = 2, Title = "Walk the dog", IsCompleted = true },
        new { Id = 3, Title = "Read a book", IsCompleted = false }
    };

    public Task<int> ExecuteAsync(string command, object? parameters = null)
    {
        try
        {
            if (command.StartsWith("INSERT") && parameters != null)
            {
                var newTodo = new
                {
                    Id = _todos.Count + 1,
                    Title = parameters.GetType().GetProperty("Title")?.GetValue(parameters, null),
                    IsCompleted = parameters.GetType().GetProperty("IsCompleted")?.GetValue(parameters, null)
                };
                _todos.Add(newTodo);
                return Task.FromResult(0);
            }
            else if (command.StartsWith("UPDATE") && parameters != null)
            {
                var id = (int)parameters.GetType().GetProperty("Id")?.GetValue(parameters, null)!;
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    todo.Title = parameters.GetType().GetProperty("Title")?.GetValue(parameters, null);
                    todo.IsCompleted = parameters.GetType().GetProperty("IsCompleted")?.GetValue(parameters, null);
                }
                return Task.FromResult(0);
            }
            else if (command.StartsWith("DELETE") && parameters != null)
            {
                var id = (int)parameters.GetType().GetProperty("Id")?.GetValue(parameters, null)!;
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    _todos.Remove(todo);
                }
                return Task.FromResult(0);
            }
            else if (command.StartsWith("SELECT"))
            {
                if (command.Contains("WHERE Id = @Id") && parameters != null)
                {
                    var id = (int)parameters.GetType().GetProperty("Id")?.GetValue(parameters, null)!;
                    var todo = _todos.FirstOrDefault(t => t.Id == id);
                    return Task.FromResult(0);
                }
                else
                {
                    return Task.FromResult(1);
                }
            }
        }
        catch
        {
            // Ignore errors in mock
        }
        return Task.FromResult(1);
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
    {
        if (query.StartsWith("SELECT"))
        {
            var result = _todos.Cast<T>();
            return Task.FromResult(result);
        }
        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<T?> QuerySingleAsync<T>(string command, object? parameters = null)
    {
        if (command.StartsWith("SELECT") && parameters != null)
        {
            var id = (int)parameters.GetType().GetProperty("Id")?.GetValue(parameters, null)!;
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            return Task.FromResult((T?)todo);
        }
        return Task.FromResult(default(T));
    }
}