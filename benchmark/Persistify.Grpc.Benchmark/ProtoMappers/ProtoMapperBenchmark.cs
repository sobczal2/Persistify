using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Persistify.Grpc.Benchmark.ProtoMappers;

[MemoryDiagnoser]
public class ProtoMapperBenchmark
{
    private ClassDtoA[] _classDtoAs;

    private StructDtoA[] _structDtoAs;

    [Params(1, 100, 10000)] public int Count { get; set; }

    private StructDtoA GenerateStructDtoA()
    {
        return new StructDtoA
        {
            A = 1,
            B = 2,
            C = "3",
            D = true,
            E = 4.0,
            F = 5.0f
        };
    }

    private ClassDtoA GenerateClassDtoA()
    {
        return new ClassDtoA
        {
            A = 1,
            B = 2,
            C = "3",
            D = true,
            E = 4.0,
            F = 5.0f
        };
    }

    private IEnumerable<T> GenerateMultiple<T>(int count, Func<T> generator)
    {
        for (var i = 0; i < count; i++) yield return generator();
    }

    [GlobalSetup]
    public void Setup()
    {
        _structDtoAs = GenerateMultiple(Count, GenerateStructDtoA).ToArray();
        _classDtoAs = GenerateMultiple(Count, GenerateClassDtoA).ToArray();
    }

    private StructDtoB MapStructDtoAsToStructDtoBs(StructDtoA dto)
    {
        return new StructDtoB
        {
            A = dto.A,
            B = dto.B,
            C = dto.C,
            D = dto.D,
            E = dto.E,
            F = dto.F
        };
    }

    private ClassDtoB MapClassDtoAsToClassDtoBs(ClassDtoA dto)
    {
        return new ClassDtoB
        {
            A = dto.A,
            B = dto.B,
            C = dto.C,
            D = dto.D,
            E = dto.E,
            F = dto.F
        };
    }

    [Benchmark]
    public object MapStructDtoAsToStructDtoBs()
    {
        var result = new StructDtoB[Count];
        for (var i = 0; i < Count; i++) result[i] = MapStructDtoAsToStructDtoBs(_structDtoAs[i]);
        return result;
    }

    [Benchmark]
    public object MapClassDtoAsToClassDtoBs()
    {
        var result = new ClassDtoB[Count];
        for (var i = 0; i < Count; i++) result[i] = MapClassDtoAsToClassDtoBs(_classDtoAs[i]);
        return result;
    }

    [Benchmark]
    public object MapStructDtoAsToStructDtoBsFast()
    {
        var structDtoBs = new StructDtoB[Count];
        ref var structDtoAStart = ref MemoryMarshal.GetArrayDataReference(_structDtoAs);
        ref var structDtoAEnd = ref Unsafe.Add(ref structDtoAStart, Count);
        ref var structDtoB = ref MemoryMarshal.GetArrayDataReference(structDtoBs);

        while (!Unsafe.AreSame(ref structDtoAStart, ref structDtoAEnd))
        {
            structDtoB = MapStructDtoAsToStructDtoBs(structDtoAStart);
            structDtoAStart = ref Unsafe.Add(ref structDtoAStart, 1);
            structDtoB = ref Unsafe.Add(ref structDtoB, 1);
        }

        return structDtoBs;
    }

    [Benchmark]
    public object MapClassDtoAsToClassDtoBsFast()
    {
        var classDtoBs = new ClassDtoB[Count];
        ref var classDtoAStart = ref MemoryMarshal.GetArrayDataReference(_classDtoAs);
        ref var classDtoAEnd = ref Unsafe.Add(ref classDtoAStart, Count);
        ref var classDtoB = ref MemoryMarshal.GetArrayDataReference(classDtoBs);

        while (!Unsafe.AreSame(ref classDtoAStart, ref classDtoAEnd))
        {
            classDtoB = MapClassDtoAsToClassDtoBs(classDtoAStart);
            classDtoAStart = ref Unsafe.Add(ref classDtoAStart, 1);
            classDtoB = ref Unsafe.Add(ref classDtoB, 1);
        }

        return classDtoBs;
    }

    public struct StructDtoA
    {
        public long A { get; set; }
        public int B { get; set; }
        public string C { get; set; }
        public bool D { get; set; }
        public double E { get; set; }
        public float F { get; set; }
    }

    public struct StructDtoB
    {
        public long A { get; set; }
        public int B { get; set; }
        public string C { get; set; }
        public bool D { get; set; }
        public double E { get; set; }
        public float F { get; set; }
    }

    public class ClassDtoA
    {
        public long A { get; set; }
        public int B { get; set; }
        public string C { get; set; }
        public bool D { get; set; }
        public double E { get; set; }
        public float F { get; set; }
    }

    public class ClassDtoB
    {
        public long A { get; set; }
        public int B { get; set; }
        public string C { get; set; }
        public bool D { get; set; }
        public double E { get; set; }
        public float F { get; set; }
    }
}