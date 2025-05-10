using System.Data;
using Bookify.Application.Abstractions.Data;
using Npgsql;

namespace Bookify.Infrastructure.Database;
internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
    
    public async Task<IDbConnection> CreateOpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
