using System.Collections.Generic;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Common;

public abstract class Validator<T> : IValidator<T>
{
    public Validator()
    {
        PropertyNames = new Stack<string>();
    }

    public Stack<string> PropertyNames { get; set; }
    public abstract Result Validate(T value);

    protected ValidationException ValidationException(string message)
    {
        return new ValidationException(string.Join('.', PropertyNames), message);
    }
}
