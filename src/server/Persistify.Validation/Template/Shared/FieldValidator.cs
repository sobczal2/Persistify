using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Shared;

public class FieldValidator : IValidator<Field>
{
    public Result<Unit> Validate(Field value)
    {
        if (string.IsNullOrEmpty(value.Name))
            return new Result<Unit>(new ValidationException("Field.Name", "Name is required"));

        if (value.Name.Length > 64)
            return new Result<Unit>(new ValidationException("Field.Name", "Name must be less than 50 characters"));

        return new Result<Unit>(Unit.Value);
    }
}
