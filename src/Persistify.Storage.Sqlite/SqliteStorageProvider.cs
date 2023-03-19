using Microsoft.Data.Sqlite;
using Persistify.Core.Storage;

namespace Persistify.Storage.Sqlite;

public partial class SqliteStorageProvider : IStorageProvider
{
    private readonly string _connectionString;

    public SqliteStorageProvider(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<string[]> GetTypes()
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"SELECT name FROM {TableNames.Types()}";
        var reader = await command.ExecuteReaderAsync();
        var types = new List<string>();
        while (await reader.ReadAsync())
        {
            types.Add(reader.GetString(0));
        }
        return types.ToArray();
    }

    public async Task Initialize()
    {
        await CreateTypesTable();
        var types = await GetTypes();
        foreach (var type in types)
        {
            await CreateIndexesTable(type);
            await CreateRecordsTable(type);
            await CreateIndexesRecordsTable(type);
        }
    }
    
    private SqliteConnection GetConnection()
    {
        var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();
        return sqliteConnection;
    }
}