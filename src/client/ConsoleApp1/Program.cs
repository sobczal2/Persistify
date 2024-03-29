﻿using System.Reflection;
using ConsoleApp1;
using Persistify.Client.HighLevel.Extensions;
using Persistify.Client.LowLevel.Core;

var client = PersistifyClientBuilder
    .Create()
    .WithBaseUrl(new Uri("http://localhost:5000"))
    .WithConnectionSettings(ConnectionSettings.NoTls)
    .BuildHighLevel();

(await client.InitializeAsync(Assembly.GetExecutingAssembly())).OnFailure(
    error => Console.WriteLine(error.Message)
);

var id = 0;


for (var j = 0; j < 1_000; j++)
{
    id = await client.AddAsync(
        new Animal
        {
            Name =
                $"Bobby {j} and his friends have a very long name that is longer than 100 characters and is used to test the full text search",
            Age = 5,
            IsAlive = true,
            BirthDate = DateTime.UtcNow,
            Picture = new byte[] { 1, 2, 3, 4, 5 }
        }
    );

    Console.WriteLine($"Added animal with id {id}");
}

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
