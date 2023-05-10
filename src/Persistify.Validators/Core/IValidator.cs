namespace Persistify.Validators.Core;

public interface IValidator<in T>
{
    ValidationFailure[] Validate(T instance);
}