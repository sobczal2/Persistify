namespace Persistify.Indexer.Types;

public class TypeField
{
    public TypeField(string path)
    {
        Path = path;
    }

    public string Path { get; set; }
}