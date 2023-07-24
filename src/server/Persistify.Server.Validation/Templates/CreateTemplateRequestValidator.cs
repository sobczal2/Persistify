using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Templates;

public class CreateTemplateRequestValidator : IValidator<CreateTemplateRequest>
{
    private readonly IValidator<BoolField> _boolFieldValidator;
    private readonly IValidator<NumberField> _numberFieldValidator;
    private readonly IValidator<TextField> _textFieldValidator;

    public CreateTemplateRequestValidator(
        IValidator<TextField> textFieldValidator,
        IValidator<NumberField> numberFieldValidator,
        IValidator<BoolField> boolFieldValidator
    )
    {
        _textFieldValidator = textFieldValidator;
        _numberFieldValidator = numberFieldValidator;
        _boolFieldValidator = boolFieldValidator;
        ErrorPrefix = "CreateTemplateRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(CreateTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName", "TemplateName is required");
        }

        if (value.TemplateName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName", "TemplateName has a maximum length of 64 characters");
        }

        if (value.TextFields.Count + value.NumberFields.Count + value.BoolFields.Count == 0)
        {
            return new ValidationException($"{ErrorPrefix}", "At least one field is required");
        }

        for (var i = 0; i < value.TextFields.Count; i++)
        {
            _textFieldValidator.ErrorPrefix = $"{ErrorPrefix}.TextFields[{i}]";
            var textFieldValidationResult = _textFieldValidator.Validate(value.TextFields[i]);
            if (textFieldValidationResult.IsFailure)
            {
                return textFieldValidationResult;
            }
        }

        for (var i = 0; i < value.NumberFields.Count; i++)
        {
            _numberFieldValidator.ErrorPrefix = $"{ErrorPrefix}.NumberFields[{i}]";
            var numberFieldValidationResult = _numberFieldValidator.Validate(value.NumberFields[i]);
            if (numberFieldValidationResult.IsFailure)
            {
                return numberFieldValidationResult;
            }
        }

        for (var i = 0; i < value.BoolFields.Count; i++)
        {
            _boolFieldValidator.ErrorPrefix = $"{ErrorPrefix}.BoolFields[{i}]";
            var boolFieldValidationResult = _boolFieldValidator.Validate(value.BoolFields[i]);
            if (boolFieldValidationResult.IsFailure)
            {
                return boolFieldValidationResult;
            }
        }

        return Result.Success;
    }
}
