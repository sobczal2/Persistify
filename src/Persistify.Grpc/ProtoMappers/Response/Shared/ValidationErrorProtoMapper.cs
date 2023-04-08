using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.Grpc.ProtoMappers.Response.Shared;

public class ValidationErrorProtoMapper : ProtoMapper<ValidationErrorProto, ValidationErrorDto>
{
    public override ValidationErrorProto MapToProto(ValidationErrorDto dto)
    {
        return new ValidationErrorProto
        {
            Field = dto.Field,
            Message = dto.Message
        };
    }

    public override ValidationErrorDto MapToDto(ValidationErrorProto proto)
    {
        return new ValidationErrorDto
        {
            Field = proto.Field,
            Message = proto.Message
        };
    }
}