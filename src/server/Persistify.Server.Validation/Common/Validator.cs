using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Shared;

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
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        return await ValidateNotNullAsync(value);
    }

    public abstract ValueTask<Result> ValidateNotNullAsync(T value);

    protected ValidationException ValidationException(string message)
    {
        var reversedPropertyNames = new List<string>(PropertyName);
        reversedPropertyNames.Reverse();
        return new ValidationException(string.Join('.', reversedPropertyNames), message);
    }
}
