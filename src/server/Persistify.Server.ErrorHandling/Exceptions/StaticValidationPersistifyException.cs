using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class StaticValidationPersistifyException : PersistifyException
{
    public StaticValidationPersistifyException(string propertyName, string message) : base(propertyName, message,
        PersistifyErrorCode.StaticValidationFailure)
    {
    }
}
