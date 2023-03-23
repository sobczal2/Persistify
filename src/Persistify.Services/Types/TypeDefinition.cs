namespace Persistify.Indexer.Types;

public class TypeDefinition
{
    public string Name { get; set; }
    public TypeField[] TypeFields { get; set; }

    public TypeDefinition(string name, TypeField[] typeFields)
    {
        Name = name;
        TypeFields = typeFields;
    }
}