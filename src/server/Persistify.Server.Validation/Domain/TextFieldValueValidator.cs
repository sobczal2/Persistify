using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValueValidator : Validator<TextFieldValue>
{
    public TextFieldValueValidator()
    {
        PropertyNames.Push(nameof(TextFieldValue));
    }

    public override Result Validate(TextFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(TextFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(TextFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
