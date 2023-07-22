using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Validation.Common;

namespace Persistify.Validation.Domain;

public class TextFieldValueValidator : IValidator<TextFieldValue>
{
    public TextFieldValueValidator()
    {
        ErrorPrefix = "TextFieldValue";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(TextFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            return new ValidationException($"{ErrorPrefix}.Value", "Value must not be empty");
        }

        if (value.FieldName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Value", "Value must not be longer than 64 characters");
        }

        return Result.Success;
    }
}
