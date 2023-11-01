using Persistify.Client.HighLevel.Attributes;

namespace ConsoleApp1;

[PersistifyDocument("Animal")]
public class Animal
{
    [PersistifyTextField]
    public string Name { get; set; } = default!;

    [PersistifyNumberField]
    public int Age { get; set; }

    [PersistifyBoolField]
    public bool IsAlive { get; set; }

    [PersistifyDateTimeField]
    public DateTime BirthDate { get; set; }

    [PersistifyBinaryField]
    public byte[] Picture { get; set; } = default!;
}
