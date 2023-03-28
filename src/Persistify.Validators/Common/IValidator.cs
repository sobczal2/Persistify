namespace Persistify.Validators.Common;

public interface IValidator<in T>
{
    ValidationResult Validate(T obj);
}
