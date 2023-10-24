
using System.Reflection;
using ConsoleApp1;
using Persistify.Client.HighLevel;
using Persistify.Client.LowLevel.Core;

var client = PersistifyClientBuilder
    .Create()
    .BuildHighLevel();

(await client.InitializeAsync(Assembly.GetExecutingAssembly()))
    .OnFailure(error => Console.WriteLine(error.Message));

(await client.AddAsync(new Animal
    {
        Name = "Bobby",
        Species = "Dog",
        Age = 5,
        Weight = 10,
        Height = 0.5,
        Color = "Brown",
        FavoriteToy = "Ball"
    }))
    .OnFailure(error => Console.WriteLine(error.Message));
