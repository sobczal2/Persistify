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
        ErrorPrefix = "Template";
    }

    public string ErrorPrefix { get; set; }


    public Result<Unit> Validate(Protos.Templates.Shared.Template value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Template is null");
        }

        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name",
                "Name's length must be lower than or equal to 64 characters");
        }

        for (var i = 0; i < value.Fields.Length; i++)
        {
            _fieldValidator.ErrorPrefix = $"{ErrorPrefix}.Fields[{i}]";

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
                return new ValidationException($"{ErrorPrefix}.Fields", $"Duplicate field name: {field.Name}");
            }

            fieldNames.Add(field.Name);
        }

        return new Result<Unit>(Unit.Value);
    }
}
