using Persistify.DataStructures.Tries;
using Persistify.DataStructures.Tries.Abstractions;

namespace Persistify.DataStructures.Benchmark.Trie.DictionaryTrie;

public class DictionaryTrieAddBenchmark : TrieAddBenchmarkBase
{
    public override ITrie<int> GetTrie()
    {
        return new DictionaryTrie<int>();
    }
}