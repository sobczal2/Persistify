using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class ByteTranslationFixedSizeTrieBenchmark
{
    private const string DataFilePath = "/home/sobczal/RiderProjects/Persistify/data/1_000_000_words.txt";
    private ITrie<int> _trie;
    private List<string> _keys;
    private List<string> _prefixes;

    [Params(1_000_000, 10_000_000)] public int TrieSize;
    [Params(100)] public int SearchCount;

    [GlobalSetup]
    public void Setup()
    {
        _trie = new ByteTranslationFixedSizeTrie<int>(c => (byte) (c - 'a'), 26);
        _keys = new List<string>();
        _prefixes = new List<string>();

        using var sr = new StreamReader(DataFilePath);
        for (var i = 0; i < TrieSize; i++)
        {
            var line = sr.ReadLine();
            if (line == null) break;

            _keys.Add(line);
            _trie.Add(line, i);

            _prefixes.Add(line[..Random.Shared.Next(8)]);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Get")]
    public void Get()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.Get(_keys[Random.Shared.Next(TrieSize)]).ToList();
    }

    [Benchmark]
    [BenchmarkCategory("Search")]
    public void Search()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.Search(_prefixes[Random.Shared.Next(TrieSize)]).ToList();
    }

    [Benchmark]
    [BenchmarkCategory("Contains")]
    public void Contains()
    {
        for(var i = 0; i < SearchCount; i++)
            _ = _trie.Contains(_keys[Random.Shared.Next(TrieSize)]);
    }
}