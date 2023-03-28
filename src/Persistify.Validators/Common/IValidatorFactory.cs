namespace Persistify.Validators.Common;

public interface IValidatorFactory
{
    IValidator<T>? GetValidator<T>();
}
