using System;
using BenchmarkDotNet.Running;
using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<ByteTranslationFixedSizeTrieBenchmark>();
    }
}