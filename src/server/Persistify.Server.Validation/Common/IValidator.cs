using System.Collections.Generic;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Common;

public interface IValidator<in T>
{
    Stack<string> PropertyName { get; set; }
    Result Validate(T value);
}
