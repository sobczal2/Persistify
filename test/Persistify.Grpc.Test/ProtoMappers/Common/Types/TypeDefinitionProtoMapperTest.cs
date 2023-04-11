using Persistify.Dtos.Common.Types;
using Persistify.ProtoMappers.Common.Types;
using Persistify.Protos;
using Xunit;

namespace Persistify.Grpc.Test.ProtoMappers.Common.Types;

public class
    TypeDefinitionProtoMapperTest : ProtoMapperTestBase<TypeDefinitionProtoMapper, TypeDefinitionProto,
        TypeDefinitionDto>
{
    protected override TypeDefinitionDto CreateDto()
    {
        return new TypeDefinitionDto
        {
            TypeName = "Test",
            Fields = new[]
            {
                new FieldDefinitionDto
                {
                    Path = "Test",
                    Type = FieldTypeDto.Text,
                    IsRequired = true
                }
            },
            IdFieldPath = "Test"
        };
    }

    protected override TypeDefinitionProto CreateProto()
    {
        return new TypeDefinitionProto
        {
            TypeName = "Test",
            Fields =
            {
                new FieldDefinitionProto
                {
                    Path = "Test",
                    Type = FieldTypeProto.Text,
                    IsRequired = true
                }
            },
            IdFieldPath = "Test"
        };
    }

    protected override void AssertAreEqual(TypeDefinitionDto dto, TypeDefinitionProto proto)
    {
        Assert.Equal(dto.TypeName, proto.TypeName);
        Assert.Equal(dto.Fields.Length, proto.Fields.Count);
        Assert.Equal(dto.Fields[0].Path, proto.Fields[0].Path);
        Assert.Equal((int)dto.Fields[0].Type, (int)proto.Fields[0].Type);
        Assert.Equal(dto.Fields[0].IsRequired, proto.Fields[0].IsRequired);
        Assert.Equal(dto.IdFieldPath, proto.IdFieldPath);
    }
}