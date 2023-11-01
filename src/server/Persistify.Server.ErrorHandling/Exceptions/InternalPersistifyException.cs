using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class InternalPersistifyException : PersistifyException
{
    public InternalPersistifyException(string? requestName = null, string message = "Unknown error")
        : base(requestName ?? "Unknown", message, PersistifyErrorCode.InternalFailure)
    {
    }
}
