using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Helpers.Results;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValueValidator : Validator<BoolFieldValue>
{
    public BoolFieldValueValidator()
    {
        PropertyName.Push(nameof(BoolFieldValue));
    }

    public override ValueTask<Result> ValidateNotNullAsync(BoolFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(BoolFieldValue.FieldName));
            return ValueTask.FromResult<Result>(StaticValidationException(DocumentErrorMessages.NameEmpty));
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(BoolFieldValue.FieldName));
            return ValueTask.FromResult<Result>(StaticValidationException(DocumentErrorMessages.NameTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
