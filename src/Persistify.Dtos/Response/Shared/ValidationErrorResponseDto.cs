namespace Persistify.Dtos.Response.Shared;

public class ValidationErrorResponseDto
{
    public ValidationErrorDto[] Errors { get; init; } = default!;
}