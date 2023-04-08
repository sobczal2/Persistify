using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.Grpc.ProtoMappers.Response.Shared;

public class ValidationErrorResponseProtoMapper : ProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto>
{
    private readonly IProtoMapper<ValidationErrorProto, ValidationErrorDto> _validationErrorProtoMapper;

    public ValidationErrorResponseProtoMapper(
        IProtoMapper<ValidationErrorProto, ValidationErrorDto> validationErrorProtoMapper)
    {
        _validationErrorProtoMapper = validationErrorProtoMapper;
    }

    public override ValidationErrorResponseProto MapToProto(ValidationErrorResponseDto dto)
    {
        return new ValidationErrorResponseProto
        {
            Errors = { _validationErrorProtoMapper.MapToProto(dto.Errors) }
        };
    }

    public override ValidationErrorResponseDto MapToDto(ValidationErrorResponseProto proto)
    {
        return new ValidationErrorResponseDto
        {
            Errors = _validationErrorProtoMapper.MapToDtoRF(proto.Errors)
        };
    }
}