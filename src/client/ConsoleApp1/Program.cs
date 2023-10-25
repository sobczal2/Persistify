using System.Reflection;
using ConsoleApp1;
using Persistify.Client.HighLevel;
using Persistify.Client.LowLevel.Core;

var client = PersistifyClientBuilder
    .Create()
    .BuildHighLevel();

(await client.InitializeAsync(Assembly.GetExecutingAssembly()))
    .OnFailure(error => Console.WriteLine(error.Message));

int id = await client.AddAsync(new Animal
{
    Name = "Bobby",
    Species = "Dog",
    Age = 5,
    Weight = 10,
    Height = 0.5,
    Color = "Brown",
    FavoriteToy = "Ball"
});

Console.WriteLine($"Added animal with id {id}");

var animal = await client.GetAsync<Animal>(id);

Console.WriteLine($"Retrieved animal with id {id}: {animal.Value}");

await client.DeleteAsync<Animal>(id);

Console.WriteLine($"Deleted animal with id {id}");
