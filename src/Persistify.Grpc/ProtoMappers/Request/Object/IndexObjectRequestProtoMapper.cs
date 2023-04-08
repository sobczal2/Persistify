using Persistify.ExternalDtos.Request.Object;

namespace Persistify.Grpc.ProtoMappers.Request.Object;

public class IndexObjectRequestProtoMapper : ProtoMapper<IndexObjectRequestProto, IndexObjectRequestDto>
{
    public override IndexObjectRequestProto MapToProto(IndexObjectRequestDto dto)
    {
        return new IndexObjectRequestProto
        {
            TypeName = dto.TypeName,
            Data = dto.Data
        };
    }

    public override IndexObjectRequestDto MapToDto(IndexObjectRequestProto proto)
    {
        return new IndexObjectRequestDto
        {
            TypeName = proto.TypeName,
            Data = proto.Data
        };
    }
}