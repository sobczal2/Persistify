using System;
using Google.Protobuf.Collections;
using Persistify.ProtoMappers;
using Xunit;

namespace Persistify.Grpc.Test.ProtoMappers;

public abstract class ProtoMapperTestBase<TProtoMapper, TProto, TDto> where TProtoMapper : ProtoMapper<TProto, TDto>
{
    protected virtual TProtoMapper ProtoMapper => Activator.CreateInstance<TProtoMapper>();

    protected abstract TDto CreateDto();
    protected abstract TProto CreateProto();

    protected abstract void AssertAreEqual(TDto dto, TProto proto);

    protected void AssertAreEqual(TDto[] dtos, TProto[] protos)
    {
        for (var i = 0; i < dtos.Length; i++) AssertAreEqual(dtos[i], protos[i]);
    }

    protected void AssertAreEqual(TDto[] dtos, RepeatedField<TProto> protos)
    {
        for (var i = 0; i < dtos.Length; i++) AssertAreEqual(dtos[i], protos[i]);
    }

    protected TDto[] CreateDtos(int count)
    {
        var dtos = new TDto[count];
        for (var i = 0; i < count; i++) dtos[i] = CreateDto();

        return dtos;
    }

    protected TProto[] CreateProtos(int count)
    {
        var protos = new TProto[count];
        for (var i = 0; i < count; i++) protos[i] = CreateProto();

        return protos;
    }

    [Fact]
    public void MapToProto_SingleDto_CorrectProto()
    {
        var dto = CreateDto();
        var proto = ProtoMapper.MapToProto(dto);
        AssertAreEqual(dto, proto);
    }

    [Fact]
    public void MapToDto_SingleProto_CorrectDto()
    {
        var proto = CreateProto();
        var dto = ProtoMapper.MapToDto(proto);
        AssertAreEqual(dto, proto);
    }

    [Fact]
    public void MapToProto_MultipleDtos_CorrectProtos()
    {
        var dtos = CreateDtos(10);
        var protos = ProtoMapper.MapToProto(dtos);
        AssertAreEqual(dtos, protos);
    }

    [Fact]
    public void MapToDto_MultipleProtos_CorrectDtos()
    {
        var protos = CreateProtos(10);
        var dtos = ProtoMapper.MapToDto(protos);
        AssertAreEqual(dtos, protos);
    }
}