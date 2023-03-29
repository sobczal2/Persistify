using System.IO;
using BenchmarkDotNet.Attributes;
using Persistify.DataStructures.Tries.Abstractions;

namespace Persistify.DataStructures.Benchmark.Trie;

[MemoryDiagnoser]
public abstract class TrieAddBenchmarkBase
{
    private const string DataFilePath = "/home/sobczal/RiderProjects/Persistify/data/1_000_000_words.txt";

    [Params(1_000, 10_000)] public static int AddedItemsCount;

    public (string Key, int Item)[] ItemsToAdd;
    public abstract ITrie<int> GetTrie();

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
        var trie = GetTrie();
        for (var i = 0; i < AddedItemsCount; i++) trie.Add(ItemsToAdd[i].Key, ItemsToAdd[i].Item);
    }
}