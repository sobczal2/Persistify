using System;
using Persistify.Extensions;
using Persistify.ExternalDtos.Common.Types;
using Persistify.Grpc.ProtoMappers.Common.Types;
using Xunit;

namespace Persistify.Grpc.Test.ProtoMappers.Common.Types;

public class
    FieldDefinitionProtoMapperTest : ProtoMapperTestBase<FieldDefinitionProtoMapper, FieldDefinitionProto,
        FieldDefinitionDto>
{
    protected override FieldDefinitionDto CreateDto()
    {
        return new FieldDefinitionDto
        {
            Path = Random.Shared.NextString(10),
            Type = Random.Shared.NextEnum<FieldTypeDto>(),
            IsRequired = Random.Shared.NextBool()
        };
    }

    protected override FieldDefinitionProto CreateProto()
    {
        return new FieldDefinitionProto
        {
            Path = Random.Shared.NextString(10),
            Type = Random.Shared.NextEnum<FieldTypeProto>(),
            IsRequired = Random.Shared.NextBool()
        };
    }

    protected override void AssertAreEqual(FieldDefinitionDto dto, FieldDefinitionProto proto)
    {
        Assert.Equal(dto.Path, proto.Path);
        Assert.Equal((int)dto.Type, (int)proto.Type);
        Assert.Equal(dto.IsRequired, proto.IsRequired);
    }
}