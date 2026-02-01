namespace Nautilus.Api.Clients;

public interface ISqlClient
{
    Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null);
    Task<T?> QuerySingleAsync<T>(string command, object? parameters = null);
    Task<int> ExecuteAsync(string command, object? parameters = null);
}