using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Google.Protobuf.Collections;

namespace Persistify.ProtoMappers;

public abstract class ProtoMapper<TProto, TDto> : IProtoMapper<TProto, TDto>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract TProto MapToProto(TDto dto);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract TDto MapToDto(TProto proto);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TProto[] MapToProto(TDto[] dtos)
    {
        var protos = new TProto[dtos.Length];
        ref var dtoStart = ref MemoryMarshal.GetArrayDataReference(dtos);
        ref var dtoEnd = ref Unsafe.Add(ref dtoStart, dtos.Length);
        ref var proto = ref MemoryMarshal.GetArrayDataReference(protos);

        while (!Unsafe.AreSame(ref dtoStart, ref dtoEnd))
        {
            proto = MapToProto(dtoStart);
            dtoStart = ref Unsafe.Add(ref dtoStart, 1);
            proto = ref Unsafe.Add(ref proto, 1);
        }

        return protos;
    }

    public TDto[] MapToDtoRF(RepeatedField<TProto> protos)
    {
        var dtos = new TDto[protos.Count];
        for (var i = 0; i < protos.Count; i++) dtos[i] = MapToDto(protos[i]);

        return dtos;
    }

    public RepeatedField<TProto> MapToProtoRF(TDto[] dtos)
    {
        var protos = new RepeatedField<TProto>();
        for (var i = 0; i < dtos.Length; i++) protos.Add(MapToProto(dtos[i]));

        return protos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TDto[] MapToDto(TProto[] protos)
    {
        var dtos = new TDto[protos.Length];
        ref var protoStart = ref MemoryMarshal.GetArrayDataReference(protos);
        ref var protoEnd = ref Unsafe.Add(ref protoStart, protos.Length);
        ref var dto = ref MemoryMarshal.GetArrayDataReference(dtos);

        while (!Unsafe.AreSame(ref protoStart, ref protoEnd))
        {
            dto = MapToDto(protoStart);
            protoStart = ref Unsafe.Add(ref protoStart, 1);
            dto = ref Unsafe.Add(ref dto, 1);
        }

        return dtos;
    }
}