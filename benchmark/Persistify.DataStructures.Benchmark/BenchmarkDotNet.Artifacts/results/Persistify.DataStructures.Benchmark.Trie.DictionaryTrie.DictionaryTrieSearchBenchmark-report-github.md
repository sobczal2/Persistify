``` ini

BenchmarkDotNet=v0.13.5, OS=fedora 37
11th Gen Intel Core i5-1135G7 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2


```
|             Method | IndexesToSearch | TrieSize |      Mean |    Error |   StdDev |   Gen0 | Allocated |
|------------------- |---------------- |--------- |----------:|---------:|---------:|-------:|----------:|
|            **Get_Key** |              **25** |     **1000** | **232.00 ns** | **1.370 ns** | **1.144 ns** |      **-** |         **-** |
|      Get_PrefixKey |              25 |     1000 | 310.32 ns | 3.204 ns | 2.997 ns | 0.0381 |     160 B |
|       Contains_Key |              25 |     1000 | 242.49 ns | 1.392 ns | 1.162 ns | 0.0057 |      24 B |
| Contains_PrefixKey |              25 |     1000 |  41.35 ns | 0.715 ns | 0.669 ns | 0.0057 |      24 B |
|            **Get_Key** |              **25** |    **10000** | **238.84 ns** | **4.808 ns** | **5.537 ns** |      **-** |         **-** |
|      Get_PrefixKey |              25 |    10000 | 330.76 ns | 3.280 ns | 2.908 ns | 0.0381 |     160 B |
|       Contains_Key |              25 |    10000 | 240.08 ns | 2.600 ns | 2.305 ns | 0.0057 |      24 B |
| Contains_PrefixKey |              25 |    10000 |  45.17 ns | 0.951 ns | 0.843 ns | 0.0057 |      24 B |
