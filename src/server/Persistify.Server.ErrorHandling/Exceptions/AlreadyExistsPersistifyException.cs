using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class AlreadyExistsPersistifyException : PersistifyException
{
    public AlreadyExistsPersistifyException(
        string? requestName = null,
        string message = "Not found"
    )
        : base(requestName ?? "Unknown", message, PersistifyErrorCode.AlreadyExists)
    {
    }
}
