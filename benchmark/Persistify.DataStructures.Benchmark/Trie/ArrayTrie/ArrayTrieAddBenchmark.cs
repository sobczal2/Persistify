using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Persistify.DataStructures.Benchmark.Trie.ArrayTrie;

[MemoryDiagnoser, SimpleJob(id: "ArrayTrie add")]
public class ArrayTrieAddBenchmark
{
    [Params(1_000, 10_000, 100_000)]
    public static int AddedItemsCount;
    
    private const string DataFilePath = "/home/sobczal/dev/dotnet/Persistify/data/1_000_000_words.txt";

    public (string Key, int Item)[] ItemsToAdd;

    [GlobalSetup]
    public void GlobalSetup()
    {
        ItemsToAdd = new (string, int)[AddedItemsCount];
        var lines = File.ReadAllLines(DataFilePath);
        for (var i = 0; i < AddedItemsCount; i++)
        {
            var line = lines[i];
            var split = line.Split(',');
            ItemsToAdd[i] = (split[0], int.Parse(split[1]));
        }
    }

    [Benchmark]
    public void Add_Items()
    {
        var trie = new Tries.ArrayTrie<int>();
        for (var i = 0; i < AddedItemsCount; i++)
        {
            trie.Add(ItemsToAdd[i].Key, ItemsToAdd[i].Item);
        }
    }
}