using Persistify.Client.Objects.Attributes;
using Persistify.Domain.Templates;

namespace ConsoleApp1;

[PersistifyTemplate]
public partial class Animal
{
    [PersistifyTextField]
    public string Name { get; set; } = default!;

    [PersistifyNumberField]
    public int Age { get; set; }

    [PersistifyBoolField]
    public bool IsAlive { get; set; }
}

