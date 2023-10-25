using Persistify.Client.HighLevel.Attributes;

namespace ConsoleApp1;

[PersistifyDocument(name: "Animal")]
public class Animal
{
    [PersistifyTextField]
    public string Name { get; set; } = default!;

    [PersistifyTextField]
    public string Species { get; set; } = default!;

    [PersistifyNumberField]
    public int Age { get; set; }

    [PersistifyNumberField]
    public double Weight { get; set; }

    [PersistifyNumberField]
    public double Height { get; set; }

    [PersistifyTextField]
    public string Color { get; set; } = default!;

    [PersistifyTextField]
    public string FavoriteToy { get; set; } = default!;

    [PersistifyBoolField]
    public bool IsAlive { get; set; }

    [PersistifyBoolField]
    public bool IsDead { get; set; }
}



