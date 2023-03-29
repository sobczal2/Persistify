``` ini

BenchmarkDotNet=v0.13.5, OS=fedora 37
11th Gen Intel Core i5-1135G7 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2


```
|             Method | IndexesToSearch | TrieSize |      Mean |    Error |   StdDev |    Gen0 | Allocated |
|------------------- |---------------- |--------- |----------:|---------:|---------:|--------:|----------:|
|            **Get_Key** |              **25** |     **1000** |  **59.87 μs** | **0.184 μs** | **0.154 μs** |  **6.6528** |  **27.41 KB** |
|      Get_PrefixKey |              25 |     1000 | 148.14 μs | 0.374 μs | 0.331 μs | 10.2539 |  42.77 KB |
|       Contains_Key |              25 |     1000 |  60.02 μs | 0.066 μs | 0.055 μs |  6.6528 |  27.41 KB |
| Contains_PrefixKey |              25 |     1000 | 150.24 μs | 0.468 μs | 0.438 μs | 10.2539 |  42.77 KB |
|            **Get_Key** |              **25** |    **10000** |  **78.75 μs** | **1.232 μs** | **1.092 μs** |  **7.2021** |  **29.89 KB** |
|      Get_PrefixKey |              25 |    10000 | 554.01 μs | 2.562 μs | 2.397 μs | 31.2500 |    129 KB |
|       Contains_Key |              25 |    10000 |  82.01 μs | 0.120 μs | 0.106 μs |  7.2021 |  29.89 KB |
| Contains_PrefixKey |              25 |    10000 | 563.18 μs | 1.221 μs | 1.142 μs | 31.2500 |    129 KB |
