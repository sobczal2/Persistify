using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Documents.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Document.Shared;

public class NumberFieldValidator : IValidator<NumberField>
{
    public NumberFieldValidator()
    {
        ErrorPrefix = "NumberField";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(NumberField value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "NumberField is null");
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.FieldName is null)
        {
            return new ValidationException($"{ErrorPrefix}.FieldName", "FieldName is null");
        }

        if (value.FieldName.Length > 64)
        {
            return new ValidationException("TextField.FieldName",
                "FieldName's length must be lower than or equal to 64 characters");
        }

        return Result.Success;
    }
}
