namespace Persistify.Storage.Sqlite;

public static class TableNames
{
    public static string Types() => "Types";
    public static string Indexes(string type) => $"{type}_Indexes";
    public static string Records(string type) => $"{type}_Records";
    public static string IndexesRecords(string type) => $"{type}_IndexesRecords";
}