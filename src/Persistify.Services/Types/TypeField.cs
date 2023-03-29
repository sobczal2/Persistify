namespace Persistify.Indexer.Types;

public class TypeField
{
    public TypeField(string path, bool indexed)
    {
        Path = path;
        Indexed = indexed;
    }

    public string Path { get; set; }
    public bool Indexed { get; set; }
}