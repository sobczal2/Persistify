using Persistify.ExternalDtos.Response.Object;

namespace Persistify.Grpc.ProtoMappers.Response.Object;

public class
    IndexObjectSuccessResponseProtoMapper : ProtoMapper<IndexObjectSuccessResponseProto, IndexObjectSuccessResponseDto>
{
    public override IndexObjectSuccessResponseProto MapToProto(IndexObjectSuccessResponseDto dto)
    {
        return new IndexObjectSuccessResponseProto
        {
            Id = dto.Id
        };
    }

    public override IndexObjectSuccessResponseDto MapToDto(IndexObjectSuccessResponseProto proto)
    {
        return new IndexObjectSuccessResponseDto
        {
            Id = proto.Id
        };
    }
}