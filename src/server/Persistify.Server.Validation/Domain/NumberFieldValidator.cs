using System.Text;
using Microsoft.Extensions.ObjectPool;
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
        PropertyNames.Push(nameof(NumberField));
    }

    public override Result Validate(NumberField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyNames.Push(nameof(NumberField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyNames.Push(nameof(NumberField.Name));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
