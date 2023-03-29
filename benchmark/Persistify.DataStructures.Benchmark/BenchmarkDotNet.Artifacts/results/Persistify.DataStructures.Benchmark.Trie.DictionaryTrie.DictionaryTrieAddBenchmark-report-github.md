``` ini

BenchmarkDotNet=v0.13.5, OS=fedora 37
11th Gen Intel Core i5-1135G7 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.11501), X64 RyuJIT AVX2


```
|    Method |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|---------- |---------:|---------:|---------:|-------:|----------:|
| Add_Items | 24.92 ns | 0.119 ns | 0.105 ns | 0.0497 |     208 B |
