using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Request.Type;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Request.Type;

public class CreateTypeRequestProtoMapper : ProtoMapper<CreateTypeRequestProto, CreateTypeRequestDto>
{
    private readonly IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> _typeDefinitionProtoMapper;

    public CreateTypeRequestProtoMapper(IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> typeDefinitionProtoMapper)
    {
        _typeDefinitionProtoMapper = typeDefinitionProtoMapper;
    }

    public override CreateTypeRequestProto MapToProto(CreateTypeRequestDto dto)
    {
        return new CreateTypeRequestProto
        {
            TypeDefinition = _typeDefinitionProtoMapper.MapToProto(dto.TypeDefinition)
        };
    }

    public override CreateTypeRequestDto MapToDto(CreateTypeRequestProto proto)
    {
        return new CreateTypeRequestDto
        {
            TypeDefinition = _typeDefinitionProtoMapper.MapToDto(proto.TypeDefinition)
        };
    }
}