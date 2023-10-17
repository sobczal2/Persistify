using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class NotFoundPersistifyException : PersistifyException
{
    public NotFoundPersistifyException(string? requestName = null, string message = "Not found") : base(
        requestName ?? "Unknown", message, PersistifyErrorCode.NotFound)
    {
    }
}
