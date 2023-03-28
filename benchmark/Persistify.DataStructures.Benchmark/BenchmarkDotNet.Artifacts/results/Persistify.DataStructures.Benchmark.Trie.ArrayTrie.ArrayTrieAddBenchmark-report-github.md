``` ini

BenchmarkDotNet=v0.13.5, OS=ubuntu 22.04
13th Gen Intel Core i7-13700KF, 1 CPU, 24 logical and 16 physical cores
.NET SDK=7.0.202
  [Host]        : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  ArrayTrie add : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2

Job=ArrayTrie add  

```
|    Method | AddedItemsCount |         Mean |       Error |      StdDev |      Gen0 |      Gen1 |      Gen2 | Allocated |
|---------- |---------------- |-------------:|------------:|------------:|----------:|----------:|----------:|----------:|
| **Add_Items** |            **1000** |     **346.6 μs** |     **5.84 μs** |     **5.46 μs** |   **74.7070** |   **55.1758** |         **-** |   **1.12 MB** |
| **Add_Items** |           **10000** |   **6,126.0 μs** |    **45.81 μs** |    **42.85 μs** |  **726.5625** |  **703.1250** |         **-** |   **10.9 MB** |
| **Add_Items** |          **100000** | **216,523.8 μs** | **4,204.55 μs** | **5,163.57 μs** | **8666.6667** | **8333.3333** | **1666.6667** | **105.67 MB** |
