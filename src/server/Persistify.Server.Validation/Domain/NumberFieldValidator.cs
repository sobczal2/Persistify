using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class NumberFieldValidator : Validator<NumberField>
{
    public NumberFieldValidator()
    {
        PropertyName.Push(nameof(NumberField));
    }

    public override ValueTask<Result> ValidateNotNullAsync(NumberField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(NumberField.Name));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(NumberField.Name));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
