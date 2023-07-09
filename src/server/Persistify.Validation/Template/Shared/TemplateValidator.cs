using System.Collections.Generic;
using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Shared;

public class TemplateValidator : IValidator<Protos.Templates.Shared.Template>
{
    private readonly IValidator<Field> _fieldValidator;

    public TemplateValidator(IValidator<Field> fieldValidator)
    {
        _fieldValidator = fieldValidator;
    }

    public Result<Unit> Validate(Protos.Templates.Shared.Template value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException("Template", "Template is null");
        }

        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException("Template.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException("Template.Name", "Name must be less than 50 characters");
        }

        for (var i = 0; i < value.Fields.Length; i++)
        {
            var result = _fieldValidator.Validate(value.Fields[i]);
            if (result.IsFailure)
            {
                return result;
            }
        }

        var fieldNames = new HashSet<string>(value.Fields.Length);

        for (var i = 0; i < value.Fields.Length; i++)
        {
            var field = value.Fields[i];
            if (fieldNames.Contains(field.Name))
            {
                return new ValidationException("Template.Fields", $"Field with name {field.Name} already exists");
            }

            fieldNames.Add(field.Name);
        }

        return new Result<Unit>(Unit.Value);
    }
}
