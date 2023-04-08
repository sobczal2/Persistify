using Persistify.ExternalDtos.Common.Types;

namespace Persistify.Grpc.ProtoMappers.Common.Types;

public class TypeDefinitionProtoMapper : ProtoMapper<TypeDefinitionProto, TypeDefinitionDto>
{
    private readonly IProtoMapper<FieldDefinitionProto, FieldDefinitionDto> _fieldDefinitionProtoMapper;

    public TypeDefinitionProtoMapper(IProtoMapper<FieldDefinitionProto, FieldDefinitionDto> fieldDefinitionProtoMapper)
    {
        _fieldDefinitionProtoMapper = fieldDefinitionProtoMapper;
    }

    public override TypeDefinitionProto MapToProto(TypeDefinitionDto dto)
    {
        return new TypeDefinitionProto
        {
            TypeName = dto.TypeName,
            Fields = { _fieldDefinitionProtoMapper.MapToProto(dto.Fields) },
            IdFieldPath = dto.IdFieldPath
        };
    }

    public override TypeDefinitionDto MapToDto(TypeDefinitionProto proto)
    {
        return new TypeDefinitionDto
        {
            TypeName = proto.TypeName,
            Fields = _fieldDefinitionProtoMapper.MapToDtoRF(proto.Fields),
            IdFieldPath = proto.IdFieldPath
        };
    }
}