using System.IO;
using Bogus;
using Xunit;

namespace Persistify.DataGenerator;

public class DataGenerator
{
    public const string DataPath = "/home/sobczal/RiderProjects/Persistify/data/1_000_000_words.txt";

    [Fact]
    public void Generate_1_000_000_random_words()
    {
        var fileStream = File.Create(DataPath);
        var writer = new StreamWriter(fileStream);

        var faker = new Faker();

        for (var i = 0; i < 1_000_000; i++)
            writer.WriteLine(
                new string(faker.Random.Chars('A', 'z', faker.Random.Int(8, 24))));

        writer.Close();
        fileStream.Close();
    }
}