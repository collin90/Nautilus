namespace Nautilus.Api.Services;

public interface IDatabaseClient
{
    Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null);
    Task<T?> QuerySingleAsync<T>(string command, object? parameters = null);
    Task<int> ExecuteAsync(string command, object? parameters = null);
}