namespace Persistify.Options.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";
    
    public string Type { get; set; } = default!;
    public string? Path { get; set; }
}