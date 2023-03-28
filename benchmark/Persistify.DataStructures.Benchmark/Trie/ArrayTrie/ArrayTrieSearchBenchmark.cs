using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Persistify.DataStructures.Benchmark.Trie.ArrayTrie;

[MemoryDiagnoser, SimpleJob(id: "ArrayTrie 100 searches")]
public class ArrayTrieSearchBenchmark
{
    [Params(1_000, 10_000, 100_000)]
    public int TrieSize;
    
    private const string DataFilePath = "/home/sobczal/dev/dotnet/Persistify/data/1_000_000_words.txt";
    private const int IndexesToSearch = 100;
    private Tries.ArrayTrie<int> _fullTrie;
    private string[] _prefixes_to_search;
    private string[] _items_to_search;

    [GlobalSetup]
    public void Setup()
    {
        _fullTrie = new Tries.ArrayTrie<int>();
        var lines = File.ReadAllLines(DataFilePath);
        for (var i = 0; i < TrieSize; i++)
        {
            var line = lines[i];
            var split = line.Split(',');
            _fullTrie.Add(split[0], int.Parse(split[1]));
        }

        _prefixes_to_search = new string[IndexesToSearch];
        _items_to_search = new string[IndexesToSearch];
        for (var i = 0; i < IndexesToSearch; i++)
        {
            _prefixes_to_search[i] = lines[i].Substring(0, 3);
            _items_to_search[i] = lines[i];
        }
    }


    [Benchmark]
    public object Get_Key()
    {
        var dumpArray = new object[IndexesToSearch]; // to prevent JIT from optimizing out the loop
        for (var i = 0; i < IndexesToSearch; i++)
        {
            dumpArray[i] = _fullTrie.Get(_items_to_search[i]);
        }
        
        return dumpArray;
    }

    [Benchmark]
    public object Get_PrefixKey()
    {
        var dumpArray = new object[IndexesToSearch]; // to prevent JIT from optimizing out the loop
        for (var i = 0; i < IndexesToSearch; i++)
        {
            dumpArray[i] = _fullTrie.GetPrefix(_prefixes_to_search[i]);
        }

        return dumpArray;
    }

    [Benchmark]
    public object Contains_Key()
    {
        var dumpArray = new object[IndexesToSearch]; // to prevent JIT from optimizing out the loop
        for (var i = 0; i < IndexesToSearch; i++)
        {
            dumpArray[i] = _fullTrie.Get(_items_to_search[i]);
        }

        return dumpArray;
    }

    [Benchmark]
    public object Contains_PrefixKey()
    {
        var dumpArray = new object[IndexesToSearch]; // to prevent JIT from optimizing out the loop
        for (var i = 0; i < IndexesToSearch; i++)
        {
            dumpArray[i] = _fullTrie.GetPrefix(_prefixes_to_search[i]);
        }
        
        return dumpArray;
    }
}