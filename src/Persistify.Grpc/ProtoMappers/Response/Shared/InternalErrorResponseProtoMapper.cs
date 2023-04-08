using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.Grpc.ProtoMappers.Response.Shared;

public class InternalErrorResponseProtoMapper : ProtoMapper<InternalErrorResponseProto, InternalErrorResponseDto>
{
    public override InternalErrorResponseProto MapToProto(InternalErrorResponseDto dto)
    {
        return new InternalErrorResponseProto
        {
            Message = dto.Message
        };
    }

    public override InternalErrorResponseDto MapToDto(InternalErrorResponseProto proto)
    {
        return new InternalErrorResponseDto
        {
            Message = proto.Message
        };
    }
}