using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class DynamicValidationPersistifyException : PersistifyException
{
    public DynamicValidationPersistifyException(string propertyName, string message)
        : base(propertyName, message, PersistifyErrorCode.DynamicValidationFailure)
    {
    }
}
