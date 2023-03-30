using Persistify.DataStructures.Tries.Abstractions;
using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Benchmark.Trie.AlphabetSubsetTrie;

public class AlphabetSubsetTrieIntSearchBenchmark : TrieSearchBenchmarkBase<int>
{
    public override ITrie<int> GetTrie()
    {
        return new AlphabetSubsetTrie<int>();
    }

    public override int GetValue(int index)
    {
        return index;
    }
}