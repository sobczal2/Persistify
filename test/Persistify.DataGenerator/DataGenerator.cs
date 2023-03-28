using System;
using System.IO;
using System.Linq;
using Bogus;
using Xunit;

namespace Persistify.DataGenerator;

public class DataGenerator
{
    public const string DataPath = "/home/sobczal/dev/dotnet/Persistify/data";
    [Fact]
    public void Generate_1_000_000_random_words()
    {
        var path = Path.Combine(DataPath, "1_000_000_words.txt");
        var fileStream = File.Create(path);
        var writer = new StreamWriter(fileStream);

        var faker = new Faker();

        for (var i = 0; i < 1_000_000; i++)
        {
            writer.WriteLine($"{new string(faker.Random.Chars('A', 'z',faker.Random.Int(8, 24)))}, {faker.Random.Int(0)}");
        }
        
        writer.Close();
        fileStream.Close();
    }
}