namespace Persistify.Storage.Sqlite;

public partial class SqliteStorageProvider
{
    private async Task CreateTypesTable()
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableNames.Types()} (name TEXT PRIMARY KEY)";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateType(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"INSERT INTO {TableNames.Types()} (name) VALUES (@name)";
        command.Parameters.AddWithValue("@name", type);
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateIndexesTable(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableNames.Indexes(type)} (index_id INTEGER PRIMARY KEY, value TEXT)";
        await command.ExecuteNonQueryAsync();
        
        command.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS {TableNames.Indexes(type)}_value ON {TableNames.Indexes(type)} (value)";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateRecordsTable(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableNames.Records(type)} (record_id INTEGER PRIMARY KEY, record TEXT)";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateIndexesRecordsTable(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableNames.IndexesRecords(type)} (index_id INTEGER, record_id INTEGER, PRIMARY KEY (index_id, record_id))";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task<bool> TypeExists(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {TableNames.Types()} WHERE name = @name";
        command.Parameters.AddWithValue("@name", type);
        var count = (long) await command.ExecuteScalarAsync();
        return count > 0;
    }
    
    private async Task EnsureTypeExists(string type)
    {
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {TableNames.Types()} WHERE name = @name";
        command.Parameters.AddWithValue("@name", type);
        var count = (long) await command.ExecuteScalarAsync();
        if (count == 0)
        {
            await CreateType(type);
            await CreateIndexesTable(type);
            await CreateRecordsTable(type);
            await CreateIndexesRecordsTable(type);
        }
    }
}