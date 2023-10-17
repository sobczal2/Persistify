using System;
using Persistify.Server.ErrorHandling.Codes;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class PersistifyException : Exception
{
    public PersistifyException(string propertyName, string message, PersistifyErrorCode errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
        PropertyName = propertyName;
    }

    public PersistifyErrorCode ErrorCode { get; }

    public string PropertyName { get; }
    public string FormattedMessage => $"{ErrorCode}: {PropertyName} - {Message}";
}
