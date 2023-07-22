using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;

namespace Persistify.Persistence.Core.Sqlite;

public class SqliteRepository<T> : IRepository<T>
{
    private readonly string _connectionString;
    private readonly ISerializer _serializer;
    private readonly string _tableName;

    public SqliteRepository(
        string connectionString,
        string tableName,
        ISerializer serializer
    )
    {
        _connectionString = connectionString;
        _tableName = tableName;
        _serializer = serializer;
        using var connection = CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {_tableName} (Id INTEGER PRIMARY KEY, Value BLOB)";
        command.ExecuteNonQuery();
    }

    public async ValueTask WriteAsync(long id, T value)
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO {_tableName} (Id, Value) VALUES (@id, @value)";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@value", _serializer.Serialize(value));
        await command.ExecuteNonQueryAsync();
    }

    public async ValueTask<T?> ReadAsync(long id)
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT Value FROM {_tableName} WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        if (!reader.Read())
        {
            return default;
        }

        return _serializer.Deserialize<T>(reader.GetFieldValue<byte[]>(0));
    }

    public async ValueTask<IEnumerable<T>> ReadAllAsync()
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT Value FROM {_tableName}";
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<T>(reader.VisibleFieldCount);
        while (await reader.ReadAsync())
        {
            result.Add(_serializer.Deserialize<T>(reader.GetFieldValue<byte[]>(0)));
        }

        return result;
    }

    public async ValueTask<bool> ExistsAsync(long id)
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {_tableName} WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        return (long)(await command.ExecuteScalarAsync())! > 0;
    }

    public async ValueTask RemoveAsync(long id)
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {_tableName} WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }

    private SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
