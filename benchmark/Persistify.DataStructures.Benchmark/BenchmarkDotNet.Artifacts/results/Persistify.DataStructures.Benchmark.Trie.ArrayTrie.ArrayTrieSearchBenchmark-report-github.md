``` ini

BenchmarkDotNet=v0.13.5, OS=fedora 37
11th Gen Intel Core i5-1135G7 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2


```
|             Method | IndexesToSearch | TrieSize |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------- |---------------- |--------- |----------:|----------:|----------:|-------:|----------:|
|            **Get_Key** |              **25** |     **1000** |  **1.663 μs** | **0.0307 μs** | **0.0288 μs** | **0.0534** |     **224 B** |
|      Get_PrefixKey |              25 |     1000 |  4.083 μs | 0.0237 μs | 0.0222 μs | 1.5030 |    6304 B |
|       Contains_Key |              25 |     1000 |  1.611 μs | 0.0088 μs | 0.0082 μs | 0.0534 |     224 B |
| Contains_PrefixKey |              25 |     1000 |  3.914 μs | 0.0236 μs | 0.0220 μs | 1.5030 |    6304 B |
|            **Get_Key** |              **25** |    **10000** |  **1.701 μs** | **0.0306 μs** | **0.0529 μs** | **0.0534** |     **224 B** |
|      Get_PrefixKey |              25 |    10000 | 10.198 μs | 0.1476 μs | 0.1380 μs | 3.9825 |   16704 B |
|       Contains_Key |              25 |    10000 |  1.807 μs | 0.0359 μs | 0.0795 μs | 0.0534 |     224 B |
| Contains_PrefixKey |              25 |    10000 | 11.367 μs | 0.2239 μs | 0.3551 μs | 3.9825 |   16704 B |
