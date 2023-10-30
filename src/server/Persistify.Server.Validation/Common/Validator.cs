using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.Codes;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;

namespace Persistify.Server.Validation.Common;

public abstract class Validator<T> : IValidator<T>
{
    public Validator()
    {
        PropertyName = new Stack<string>();
    }

    public Stack<string> PropertyName { get; set; }

    public async ValueTask<Result> ValidateAsync(T value)
    {
        if (value == null)
        {
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        return await ValidateNotNullAsync(value);
    }

    public abstract ValueTask<Result> ValidateNotNullAsync(T value);

    protected PersistifyException StaticValidationException(string message)
    {
        var reversedPropertyNames = new List<string>(PropertyName);
        reversedPropertyNames.Reverse();
        return new StaticValidationPersistifyException(
            string.Join('.', reversedPropertyNames),
            message
        );
    }

    protected PersistifyException DynamicValidationException(string message)
    {
        var reversedPropertyNames = new List<string>(PropertyName);
        reversedPropertyNames.Reverse();
        return new DynamicValidationPersistifyException(
            string.Join('.', reversedPropertyNames),
            message
        );
    }

    protected PersistifyException Exception(string message, PersistifyErrorCode errorCode)
    {
        var reversedPropertyNames = new List<string>(PropertyName);
        reversedPropertyNames.Reverse();
        return new PersistifyException(string.Join('.', reversedPropertyNames), message, errorCode);
    }
}
