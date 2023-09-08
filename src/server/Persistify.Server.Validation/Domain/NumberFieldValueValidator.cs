using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class NumberFieldValueValidator : Validator<NumberFieldValue>
{
    public NumberFieldValueValidator()
    {
        PropertyNames.Push(nameof(NumberFieldValue));
    }

    public override Result Validate(NumberFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(NumberFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(NumberFieldValue.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
