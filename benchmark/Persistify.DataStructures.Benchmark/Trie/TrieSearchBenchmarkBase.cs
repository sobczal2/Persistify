using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Persistify.DataStructures.Tries.Abstractions;

namespace Persistify.DataStructures.Benchmark.Trie;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public abstract class TrieSearchBenchmarkBase<TItem> where TItem : struct, IComparable<TItem>, IConvertible
{
    private const string DataFilePath = "/home/sobczal/RiderProjects/Persistify/data/1_000_000_words.txt";
    private ITrie<TItem> _trie;
    private List<string> _keys;
    private List<string> _prefixes;

    [Params(1_000, 100_000)] public int TrieSize;
    [Params(100)] public int SearchCount;

    public abstract ITrie<TItem> GetTrie();
    public abstract TItem GetValue(int index);

    [GlobalSetup]
    public void Setup()
    {
        _trie = GetTrie();
        _keys = new List<string>();
        _prefixes = new List<string>();

        using var sr = new StreamReader(DataFilePath);
        for (var i = 0; i < TrieSize; i++)
        {
            var line = sr.ReadLine();
            if (line == null) break;

            _keys.Add(line);
            _trie.Add(line, GetValue(i));

            _prefixes.Add(line[..Random.Shared.Next(8)]);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Get")]
    public void Get()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.Get(_keys[Random.Shared.Next(TrieSize)]);
    }

    [Benchmark]
    [BenchmarkCategory("GetPrefix")]
    public void GetPrefix()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.GetPrefix(_prefixes[Random.Shared.Next(TrieSize)]);
    }

    [Benchmark]
    [BenchmarkCategory("Contains")]
    public void Contains()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.Contains(_keys[Random.Shared.Next(TrieSize)]);
    }

    [Benchmark]
    [BenchmarkCategory("ContainsPrefix")]
    public void ContainsPrefix()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.ContainsPrefix(_prefixes[Random.Shared.Next(TrieSize)]);
    }
}