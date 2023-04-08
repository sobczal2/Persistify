namespace Persistify.ExternalDtos.Response.Shared;

public class ValidationErrorResponseDto
{
    public ValidationErrorDto[] Errors { get; init; } = default!;
}