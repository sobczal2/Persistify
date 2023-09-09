using System.Collections.Generic;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Common;

public abstract class Validator<T> : IValidator<T>
{
    public Validator()
    {
        PropertyName = new Stack<string>();
    }

    public Stack<string> PropertyName { get; set; }

    public Result Validate(T value)
    {
        if (value == null)
        {
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        return ValidateNotNull(value);
    }
    public abstract Result ValidateNotNull(T value);

    protected ValidationException ValidationException(string message)
    {
        var reversedPropertyNames = new List<string>(PropertyName);
        reversedPropertyNames.Reverse();
        return new ValidationException(string.Join('.', reversedPropertyNames), message);
    }
}
