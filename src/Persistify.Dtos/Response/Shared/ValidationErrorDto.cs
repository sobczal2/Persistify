namespace Persistify.Dtos.Response.Shared;

public class ValidationErrorDto
{
    public string Field { get; init; } = default!;
    public string Message { get; init; } = default!;
}