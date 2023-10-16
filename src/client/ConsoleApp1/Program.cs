using System.Diagnostics;
using ConsoleApp1;
using Persistify.Client.Core;

var username = "test123456";

var client = PersistifyClientBuilder.Create()
    .WithBaseUrl(new Uri("http://localhost:5000"))
    .WithCredentials("root", "root")
    .Build();

// implicit call to SignInAsync
await AnimalHelpers.EnsureAnimalTemplateExists(client);

var sw = Stopwatch.StartNew();

for (var i = 0; i < 100; i++)
{
    var tasks = new List<Task>();

    for (var j = 0; j < 100; j++)
    {
        tasks.Add(AnimalHelpers.CreateRandomAnimal(client));
        // tasks.Add(AnimalHelpers.SearchForAnimals(client, "metal", false));
    }

    await Task.WhenAll(tasks);
}


Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
