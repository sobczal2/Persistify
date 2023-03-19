namespace Persistify.Storage.Sqlite;

public partial class SqliteStorageProvider
{
    public async Task<int> AddRecord(string type, string record)
    {
        await EnsureTypeExists(type);
        await using var connection = GetConnection();
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $"INSERT INTO {TableNames.Records(type)} (record) VALUES (@record)";
        insertCommand.Parameters.AddWithValue("@record", record);
        await insertCommand.ExecuteNonQueryAsync();
        
        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = "SELECT last_insert_rowid()";
        var id = (int)(long) await selectCommand.ExecuteScalarAsync();
        connection.Close();
        
        return id;
    }

    public async Task AddIndex(string type, string value, int recordId)
    {
        await EnsureTypeExists(type);
        await using var connection = GetConnection();
        var insertIndexCommand = connection.CreateCommand();
        insertIndexCommand.CommandText = $"INSERT OR IGNORE INTO {TableNames.Indexes(type)} (value) VALUES (@value)";
        insertIndexCommand.Parameters.AddWithValue("@value", value);
        await insertIndexCommand.ExecuteNonQueryAsync();
        
        var selectIndexCommand = connection.CreateCommand();
        selectIndexCommand.CommandText = $"SELECT index_id FROM {TableNames.Indexes(type)} WHERE value = @value";
        selectIndexCommand.Parameters.AddWithValue("@value", value);
        var indexId = (int)(long) await selectIndexCommand.ExecuteScalarAsync();
        
        var insertIndexRecordCommand = connection.CreateCommand();
        insertIndexRecordCommand.CommandText = $"INSERT INTO {TableNames.IndexesRecords(type)} (index_id, record_id) VALUES (@index_id, @record_id)";
        insertIndexRecordCommand.Parameters.AddWithValue("@index_id", indexId);
        insertIndexRecordCommand.Parameters.AddWithValue("@record_id", recordId);
        await insertIndexRecordCommand.ExecuteNonQueryAsync();
    }
}