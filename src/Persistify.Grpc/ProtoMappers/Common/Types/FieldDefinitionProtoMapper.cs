using Persistify.ExternalDtos.Common.Types;

namespace Persistify.Grpc.ProtoMappers.Common.Types;

public class FieldDefinitionProtoMapper : ProtoMapper<FieldDefinitionProto, FieldDefinitionDto>
{
    public override FieldDefinitionProto MapToProto(FieldDefinitionDto dto)
    {
        return new FieldDefinitionProto
        {
            Path = dto.Path,
            Type = (FieldTypeProto)dto.Type,
            IsRequired = dto.IsRequired
        };
    }

    public override FieldDefinitionDto MapToDto(FieldDefinitionProto proto)
    {
        return new FieldDefinitionDto
        {
            Path = proto.Path,
            Type = (FieldTypeDto)proto.Type,
            IsRequired = proto.IsRequired
        };
    }
}