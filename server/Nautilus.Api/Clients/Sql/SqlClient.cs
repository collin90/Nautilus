using Dapper;
using Microsoft.Data.SqlClient;

namespace Nautilus.Api.Clients.Sql;

public class SqlClient(IConfiguration config) : ISqlClient
{
    private readonly string _connectionString = config.GetConnectionString("NautilusDatabase") ?? throw new InvalidOperationException("Connection string 'NautilusDatabase' not found.");

    private SqlConnection Connect() => new(_connectionString);

    public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
    {
        using var connection = Connect();
        return await connection.QueryAsync<T>(query, parameters);
    }

    public async Task<T?> QuerySingleAsync<T>(string command, object? parameters = null)
    {
        using var connection = Connect();
        return await connection.QuerySingleOrDefaultAsync<T>(command, parameters);
    }

    public async Task<int> ExecuteAsync(string command, object? parameters = null)
    {
        using var connection = Connect();
        return await connection.ExecuteAsync(command, parameters);
    }

}