using Persistify.DataStructures.Tries;
using Persistify.DataStructures.Tries.Abstractions;

namespace Persistify.DataStructures.Benchmark.Trie.ArrayTrie;

public class ArrayTrieAddBenchmark : TrieAddBenchmarkBase
{
    public override ITrie<int> GetTrie()
    {
        return new ArrayTrie<int>();
    }
}