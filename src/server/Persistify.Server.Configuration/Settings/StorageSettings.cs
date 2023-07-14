namespace Persistify.Server.Configuration.Settings;

public class StorageSettings
{
    public const string SectionName = "Storage";

    public string DataPath { get; set; } = default!;
    public string KeyValueStoragePath { get; set; } = default!;
}
