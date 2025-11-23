using Nautilus.Api.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nautilus.Api.Services;

public class MockClient : IDatabaseClient
{
    // Keep a simple todo store for unrelated queries
    private readonly List<dynamic> _todos = new List<dynamic>
    {
        new { Id = 1, Title = "Buy groceries", IsCompleted = false },
        new { Id = 2, Title = "Walk the dog", IsCompleted = true },
        new { Id = 3, Title = "Read a book", IsCompleted = false }
    };

    // In-memory user store to mimic auth stored-procedure behaviour
    private readonly List<User> _users = new List<User>();

    public Task<int> ExecuteAsync(string command, object? parameters = null)
    {
        // For the purposes of the mock we don't need to support many execute commands.
        // Return 0 for success, 1 for no-op/failure.
        try
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                // Basic todo commands (retain prior mock behaviour)
                if (command.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) && parameters != null)
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

                if (command.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) && parameters != null)
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

                if (command.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) && parameters != null)
                {
                    var id = (int)parameters.GetType().GetProperty("Id")?.GetValue(parameters, null)!;
                    var todo = _todos.FirstOrDefault(t => t.Id == id);
                    if (todo != null)
                    {
                        _todos.Remove(todo);
                    }
                    return Task.FromResult(0);
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
        if (!string.IsNullOrWhiteSpace(query) && query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            // If the caller expects users, return the user list
            if (typeof(T) == typeof(User))
            {
                var result = _users.Cast<T>();
                return Task.FromResult(result);
            }

            var resultTodos = _todos.Cast<T>();
            return Task.FromResult(resultTodos);
        }

        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<T?> QuerySingleAsync<T>(string command, object? parameters = null)
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            // Handle the auth stored-procedure for registration
            if (string.Equals(command, "auth.RegisterUser", StringComparison.OrdinalIgnoreCase))
            {
                var email = parameters?.GetType().GetProperty("Email")?.GetValue(parameters, null) as string;
                var passwordHash = parameters?.GetType().GetProperty("PasswordHash")?.GetValue(parameters, null) as string;
                var passwordSalt = parameters?.GetType().GetProperty("PasswordSalt")?.GetValue(parameters, null) as string;

                if (string.IsNullOrWhiteSpace(email))
                    return Task.FromResult(default(T));

                // If a user with the same email exists, mimic DB returning null (failure)
                if (_users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
                    return Task.FromResult(default(T));

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = passwordHash ?? string.Empty,
                    PasswordSalt = passwordSalt ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _users.Add(user);

                if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
                {
                    object? outVal = user.UserId;
                    return Task.FromResult((T?)outVal);
                }

                return Task.FromResult(default(T));
            }

            // Handle login stored-procedure: return matching user by email (or null)
            if (string.Equals(command, "auth.LoginUser", StringComparison.OrdinalIgnoreCase))
            {
                var identifier = parameters?.GetType().GetProperty("UserNameOrEmail")?.GetValue(parameters, null) as string;
                if (string.IsNullOrWhiteSpace(identifier))
                    return Task.FromResult(default(T));

                // Try match by email (mock doesn't store username separately)
                var user = _users.FirstOrDefault(u => string.Equals(u.Email, identifier, StringComparison.OrdinalIgnoreCase));

                return Task.FromResult((T?)(object?)user);
            }

            // Fallback: simple SELECT by Id against todos
            if (command.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) && parameters != null)
            {
                var idProp = parameters.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    var id = (int)idProp.GetValue(parameters, null)!;
                    var todo = _todos.FirstOrDefault(t => t.Id == id);
                    return Task.FromResult((T?)todo);
                }
            }
        }

        return Task.FromResult(default(T));
    }
}