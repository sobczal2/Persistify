namespace Persistify.Common.Options;

public class PersistifyOptions
{
    public static string SectionName = "Persistify";
    public StorageProviderEnum StorageProvider { get; set; }
    public string? FileStorageProviderRoot { get; set; }
}