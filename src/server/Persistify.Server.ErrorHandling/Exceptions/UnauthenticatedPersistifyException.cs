using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class UnauthenticatedPersistifyException : PersistifyException
{
    public UnauthenticatedPersistifyException(string? requestName, string message = "Unauthenticated") : base(requestName ?? "Unknown", message, PersistifyErrorCode.Unauthenticated)
    {
    }
}
