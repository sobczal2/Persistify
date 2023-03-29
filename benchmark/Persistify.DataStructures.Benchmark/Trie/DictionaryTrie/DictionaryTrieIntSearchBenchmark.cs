using Persistify.DataStructures.Tries;
using Persistify.DataStructures.Tries.Abstractions;

namespace Persistify.DataStructures.Benchmark.Trie;

public class DictionaryTrieIntSearchBenchmark : TrieSearchBenchmarkBase<int>
{
    public override ITrie<int> GetTrie()
    {
        return new DictionaryTrie<int>();
    }

    public override int GetValue(int index)
    {
        return index;
    }
}