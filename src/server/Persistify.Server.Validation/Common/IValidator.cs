using System.Collections.Generic;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Common;

public interface IValidator<in T>
{
    Result Validate(T value);
    Stack<string> PropertyNames { get; set; }
}
