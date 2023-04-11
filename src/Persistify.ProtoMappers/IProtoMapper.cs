using System.Runtime.CompilerServices;
using Google.Protobuf.Collections;

namespace Persistify.ProtoMappers;

public interface IProtoMapper<TProto, TDto>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TProto MapToProto(TDto dto);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TDto MapToDto(TProto proto);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TProto[] MapToProto(TDto[] dtos);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TDto[] MapToDtoRF(RepeatedField<TProto> protos);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    RepeatedField<TProto> MapToProtoRF(TDto[] dtos);
}