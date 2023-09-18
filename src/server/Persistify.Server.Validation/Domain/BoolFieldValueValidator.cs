using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValueValidator : Validator<BoolFieldValue>
{
    public BoolFieldValueValidator()
    {
        PropertyName.Push(nameof(BoolFieldValue));
    }

    public override Result ValidateNotNull(BoolFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(BoolFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(BoolFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
