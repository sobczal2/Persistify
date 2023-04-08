using Persistify.ExternalDtos.Common.Types;
using Persistify.ExternalDtos.Request.Type;

namespace Persistify.Grpc.ProtoMappers.Request.Type;

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