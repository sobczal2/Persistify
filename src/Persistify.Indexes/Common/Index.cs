namespace Persistify.Indexes.Common;

public struct Index
{
    public long Id { get; set; }
    public string Path { get; set; }
    
    public Index(long id, string path)
    {
        Id = id;
        Path = path;
    }
}