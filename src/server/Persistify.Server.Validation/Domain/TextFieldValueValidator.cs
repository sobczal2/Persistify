using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValueValidator : Validator<TextFieldValue>
{
    public TextFieldValueValidator()
    {
        PropertyName.Push(nameof(TextFieldValue));
    }

    public override Result ValidateNotNull(TextFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(TextFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(TextFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
