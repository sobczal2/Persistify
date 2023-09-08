using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class CreateTemplateRequestValidator : Validator<CreateTemplateRequest>
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
        _textFieldValidator.PropertyNames = PropertyNames;
        _numberFieldValidator = numberFieldValidator;
        _numberFieldValidator.PropertyNames = PropertyNames;
        _boolFieldValidator = boolFieldValidator;
        _textFieldValidator.PropertyNames = PropertyNames;
        PropertyNames.Push(nameof(CreateTemplateRequest));
    }

    public override Result Validate(CreateTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyNames.Push(nameof(CreateTemplateRequest.TemplateName));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyNames.Push(nameof(CreateTemplateRequest.TemplateName));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        if (value.TextFields.Count + value.NumberFields.Count + value.BoolFields.Count == 0)
        {
            PropertyNames.Push("*Fields");
            return ValidationException(TemplateErrorMessages.NoFields);
        }

        for (var i = 0; i < value.TextFields.Count; i++)
        {
            PropertyNames.Push($"{nameof(CreateTemplateRequest.TextFields)}[{i}]");
            var textFieldValidationResult = _textFieldValidator.Validate(value.TextFields[i]);
            PropertyNames.Pop();
            if (textFieldValidationResult.Failure)
            {
                return textFieldValidationResult;
            }
        }

        for (var i = 0; i < value.NumberFields.Count; i++)
        {
            PropertyNames.Push($"{nameof(CreateTemplateRequest.NumberFields)}[{i}]");
            var numberFieldValidationResult = _numberFieldValidator.Validate(value.NumberFields[i]);
            PropertyNames.Pop();
            if (numberFieldValidationResult.Failure)
            {
                return numberFieldValidationResult;
            }
        }

        for (var i = 0; i < value.BoolFields.Count; i++)
        {
            PropertyNames.Push($"{nameof(CreateTemplateRequest.BoolFields)}[{i}]");
            var boolFieldValidationResult = _boolFieldValidator.Validate(value.BoolFields[i]);
            PropertyNames.Pop();
            if (boolFieldValidationResult.Failure)
            {
                return boolFieldValidationResult;
            }
        }

        return Result.Ok;
    }
}
