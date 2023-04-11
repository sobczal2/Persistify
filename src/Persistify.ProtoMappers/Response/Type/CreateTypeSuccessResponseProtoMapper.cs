using Persistify.Dtos.Response.Type;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Response.Type;

public class
    CreateTypeSuccessResponseProtoMapper : ProtoMapper<CreateTypeSuccessResponseProto, CreateTypeSuccessResponseDto>
{
    public override CreateTypeSuccessResponseProto MapToProto(CreateTypeSuccessResponseDto dto)
    {
        return new CreateTypeSuccessResponseProto();
    }

    public override CreateTypeSuccessResponseDto MapToDto(CreateTypeSuccessResponseProto proto)
    {
        return new CreateTypeSuccessResponseDto();
    }
}