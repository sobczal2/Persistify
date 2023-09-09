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
        PropertyName.Push(nameof(NumberField));
    }

    public override Result ValidateNotNull(NumberField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(NumberField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(NumberField.Name));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
