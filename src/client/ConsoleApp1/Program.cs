using System.Reflection;
using ConsoleApp1;
using Persistify.Client.HighLevel;
using Persistify.Client.LowLevel.Core;

var client = PersistifyClientBuilder
    .Create()
    .WithBaseUrl(new Uri("https://localhost:5000"))
    .BuildHighLevel();

(await client.InitializeAsync(Assembly.GetExecutingAssembly())).OnFailure(
    error => Console.WriteLine(error.Message)
);

var id = 0;

var tasks = new List<Task>();

for (var i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(async () =>
    {
        for (var j = 0; j < 100_000; j++)
        {
            id = await client.AddAsync(
                new Animal
                {
                    Name = $"Bobby {j} and his friends have a very long name that is longer than 100 characters and is used to test the full text search",
                    Species = "Dog",
                    Age = 5,
                    Weight = 10,
                    Height = 0.5,
                    Color = "Brown",
                    FavoriteToy = "Ball",
                    IsAlive = true,
                    IsDead = false
                }
            );

            Console.WriteLine($"Added animal with id {id}");
        }
    }));
}

await Task.WhenAll(tasks);

Console.WriteLine($"Added animal with id {id}");

var animal = await client.GetAsync<Animal>(id);

Console.WriteLine($"Retrieved animal with id {id}: {animal.Value}");

var searchResult = await client.SearchAsync<Animal>(
    builder =>
        builder
            .WithPagination(0, 10)
            .WithSearchQuery(
                sqBuilder =>
                    sqBuilder
                        .And()
                        .AddQuery(q => q.ExactBool().WithField(a => a.IsAlive).WithValue(true))
                        .AddQuery(q => q.ExactNumber().WithField(a => a.Age).WithValue(5))
                        .AddQuery(q => q.FullText().WithField(a => a.Name).WithValue("friends"))
            )
);

await client.DeleteAsync<Animal>(id);

Console.WriteLine($"Deleted animal with id {id}");
