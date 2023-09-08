using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValidator : Validator<BoolField>
{
    public BoolFieldValidator()
    {
        PropertyNames.Push(nameof(BoolField));
    }

    public override Result Validate(BoolField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyNames.Push(nameof(BoolField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyNames.Push(nameof(BoolField.Name));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
