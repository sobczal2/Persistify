using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Shared;

public class FieldValidator : IValidator<Field>
{
    public FieldValidator()
    {
        ErrorPrefix = "Field";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(Field value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Field is null");
        }

        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name must be less than 50 characters");
        }

        return Result.Success;
    }
}
