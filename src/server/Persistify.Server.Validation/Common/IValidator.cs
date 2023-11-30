using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Helpers.Results;

namespace Persistify.Server.Validation.Common;

public interface IValidator<in T>
{
    Stack<string> PropertyName { get; set; }

    ValueTask<Result> ValidateAsync(
        T value
    );
}
