using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
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
        _textFieldValidator = textFieldValidator ?? throw new ArgumentNullException(nameof(textFieldValidator));
        _textFieldValidator.PropertyName = PropertyName;
        _numberFieldValidator = numberFieldValidator ?? throw new ArgumentNullException(nameof(numberFieldValidator));
        _numberFieldValidator.PropertyName = PropertyName;
        _boolFieldValidator = boolFieldValidator ?? throw new ArgumentNullException(nameof(boolFieldValidator));
        _boolFieldValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(CreateTemplateRequest));
    }

    public override Result ValidateNotNull(CreateTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        if (value.TextFields.Count + value.NumberFields.Count + value.BoolFields.Count == 0)
        {
            PropertyName.Push("*Fields");
            return ValidationException(TemplateErrorMessages.NoFields);
        }

        for (var i = 0; i < value.TextFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.TextFields)}[{i}]");
            var textFieldValidationResult = _textFieldValidator.Validate(value.TextFields[i]);
            PropertyName.Pop();
            if (textFieldValidationResult.Failure)
            {
                return textFieldValidationResult;
            }
        }

        for (var i = 0; i < value.NumberFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.NumberFields)}[{i}]");
            var numberFieldValidationResult = _numberFieldValidator.Validate(value.NumberFields[i]);
            PropertyName.Pop();
            if (numberFieldValidationResult.Failure)
            {
                return numberFieldValidationResult;
            }
        }

        for (var i = 0; i < value.BoolFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.BoolFields)}[{i}]");
            var boolFieldValidationResult = _boolFieldValidator.Validate(value.BoolFields[i]);
            PropertyName.Pop();
            if (boolFieldValidationResult.Failure)
            {
                return boolFieldValidationResult;
            }
        }

        var allNames = new HashSet<string>(value.TextFields.Count + value.NumberFields.Count +
                                                value.BoolFields.Count);

        for (var i = 0; i < value.TextFields.Count; i++)
        {
            var name = value.TextFields[i].Name;
            if (allNames.Contains(name))
            {
                PropertyName.Push($"{nameof(CreateTemplateRequest.TextFields)}[{i}]");
                PropertyName.Push(nameof(TextField.Name));
                return ValidationException(TemplateErrorMessages.FieldNameNotUnique);
            }

            allNames.Add(name);
        }

        for (var i = 0; i < value.NumberFields.Count; i++)
        {
            var name = value.NumberFields[i].Name;
            if (allNames.Contains(name))
            {
                PropertyName.Push($"{nameof(CreateTemplateRequest.NumberFields)}[{i}]");
                PropertyName.Push(nameof(NumberField.Name));
                return ValidationException(TemplateErrorMessages.FieldNameNotUnique);
            }

            allNames.Add(name);
        }

        for (var i = 0; i < value.BoolFields.Count; i++)
        {
            var name = value.BoolFields[i].Name;
            if (allNames.Contains(name))
            {
                PropertyName.Push($"{nameof(CreateTemplateRequest.BoolFields)}[{i}]");
                PropertyName.Push(nameof(BoolField.Name));
                return ValidationException(TemplateErrorMessages.FieldNameNotUnique);
            }

            allNames.Add(name);
        }

        return Result.Ok;
    }
}
