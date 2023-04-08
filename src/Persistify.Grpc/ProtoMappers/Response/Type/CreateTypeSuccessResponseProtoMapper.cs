using Persistify.ExternalDtos.Response.Type;

namespace Persistify.Grpc.ProtoMappers.Response.Type;

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