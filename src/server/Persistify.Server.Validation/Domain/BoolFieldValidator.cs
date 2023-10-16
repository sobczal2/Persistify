using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValidator : Validator<BoolField>
{
    public BoolFieldValidator()
    {
        PropertyName.Push(nameof(BoolField));
    }

    public override ValueTask<Result> ValidateNotNullAsync(BoolField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(BoolField.Name));
            return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(BoolField.Name));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
