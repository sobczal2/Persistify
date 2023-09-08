using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValueValidator : Validator<BoolFieldValue>
{
    public BoolFieldValueValidator()
    {
        PropertyNames.Push(nameof(BoolFieldValue));
    }

    public override Result Validate(BoolFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(BoolFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(BoolFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
