``` ini

BenchmarkDotNet=v0.13.5, OS=fedora 37
11th Gen Intel Core i5-1135G7 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2


```
|         Method | TrieSize | SearchCount | Mean | Error |
|--------------- |--------- |------------ |-----:|------:|
|       **Contains** |     **1000** |         **100** |   **NA** |    **NA** |
|       **Contains** |   **100000** |         **100** |   **NA** |    **NA** |
|                |          |             |      |       |
| **ContainsPrefix** |     **1000** |         **100** |   **NA** |    **NA** |
| **ContainsPrefix** |   **100000** |         **100** |   **NA** |    **NA** |
|                |          |             |      |       |
|            **Get** |     **1000** |         **100** |   **NA** |    **NA** |
|            **Get** |   **100000** |         **100** |   **NA** |    **NA** |
|                |          |             |      |       |
|      **GetPrefix** |     **1000** |         **100** |   **NA** |    **NA** |
|      **GetPrefix** |   **100000** |         **100** |   **NA** |    **NA** |

Benchmarks with issues:
  ArrayTrieIntSearchBenchmark.Contains: DefaultJob [TrieSize=1000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.Contains: DefaultJob [TrieSize=100000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.ContainsPrefix: DefaultJob [TrieSize=1000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.ContainsPrefix: DefaultJob [TrieSize=100000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.Get: DefaultJob [TrieSize=1000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.Get: DefaultJob [TrieSize=100000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.GetPrefix: DefaultJob [TrieSize=1000, SearchCount=100]
  ArrayTrieIntSearchBenchmark.GetPrefix: DefaultJob [TrieSize=100000, SearchCount=100]
