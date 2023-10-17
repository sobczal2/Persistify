using Persistify.Client.Objects.Attributes;

namespace ConsoleApp1;

[PersistifyTemplate]
public class Animal
{
    [PersistifyTextField]
    public string Name { get; set; } = default!;

    [PersistifyNumberField]
    public int Age { get; set; }

    [PersistifyBoolField]
    public bool IsAlive { get; set; }
}
