using Persistify.Helpers.ErrorHandling;

namespace Persistify.Validation.Common;

public interface IValidator<in T>
{
    public Result<Unit> Validate(T value);
}
