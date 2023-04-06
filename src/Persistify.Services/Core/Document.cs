namespace Persistify.Indexer.Core;

public struct Document
{
    public long Id { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
}