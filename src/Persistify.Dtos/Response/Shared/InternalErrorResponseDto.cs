namespace Persistify.Dtos.Response.Shared;

public struct InternalErrorResponseDto
{
    public InternalErrorResponseDto(string message = "An internal error occurred.")
    {
        Message = message;
    }

    public string Message { get; init; }
}