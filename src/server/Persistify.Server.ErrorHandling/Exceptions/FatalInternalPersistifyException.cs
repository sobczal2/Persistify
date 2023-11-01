using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class FatalInternalPersistifyException : PersistifyException
{
    public FatalInternalPersistifyException(
        string? requestName = null,
        string message = "Fatal internal error"
    )
        : base(requestName ?? "Unknown", message, PersistifyErrorCode.FatalInternalFailure)
    {
    }
}
