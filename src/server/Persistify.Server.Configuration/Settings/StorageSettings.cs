namespace Persistify.Server.Configuration.Settings;

public class StorageSettings
{
    public static string SectionName => "Storage";

    public string DataPath { get; set; } = default!;
}
