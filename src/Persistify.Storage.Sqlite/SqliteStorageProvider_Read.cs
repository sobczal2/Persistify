namespace Persistify.Storage.Sqlite;

public partial class SqliteStorageProvider
{
    public async Task<(string value, int id)[]> GetIndexes(string type)
    {
        if(!await TypeExists(type))
        {
            return Array.Empty<(string value, int id)>();
        }
        await using var sqliteConnection = GetConnection();
        var countCommand = sqliteConnection.CreateCommand();
        countCommand.CommandText = $"SELECT COUNT(*) FROM {TableNames.Indexes(type)}";
        var count = (long) await countCommand.ExecuteScalarAsync();
        if (count == 0)
        {
            return Array.Empty<(string value, int id)>();
        }
        
        var readCommand = sqliteConnection.CreateCommand();
        readCommand.CommandText = $"SELECT value, index_id FROM {TableNames.Indexes(type)}";
        var reader = await readCommand.ExecuteReaderAsync();
        var indexes = new (string value, int id)[count];
        
        var i = 0;
        while (await reader.ReadAsync())
        {
            indexes[i] = (reader.GetString(0), reader.GetInt32(1));
            i++;
        }
        
        return indexes;
    }

    public async Task<string?> GetRecord(string type, int id)
    {
        if(!await TypeExists(type))
        {
            return null;
        }
        await using var sqliteConnection = GetConnection();
        var command = sqliteConnection.CreateCommand();
        command.CommandText = $"SELECT record FROM {TableNames.Records(type)} WHERE record_id = @id";
        command.Parameters.AddWithValue("@id", id);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }
}