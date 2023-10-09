using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Helpers.Results;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValueValidator : Validator<TextFieldValue>
{
    public TextFieldValueValidator()
    {
        PropertyName.Push(nameof(TextFieldValue));
    }

    public override ValueTask<Result> ValidateNotNullAsync(TextFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(TextFieldValue.FieldName));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.NameEmpty));
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(TextFieldValue.FieldName));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.NameTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
