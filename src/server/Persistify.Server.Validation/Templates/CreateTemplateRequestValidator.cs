using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class CreateTemplateRequestValidator : Validator<CreateTemplateRequest>
{
    private readonly IValidator<BoolField> _boolFieldValidator;
    private readonly IValidator<NumberField> _numberFieldValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextField> _textFieldValidator;

    public CreateTemplateRequestValidator(
        IValidator<TextField> textFieldValidator,
        IValidator<NumberField> numberFieldValidator,
        IValidator<BoolField> boolFieldValidator,
        ITemplateManager templateManager
    )
    {
        _textFieldValidator = textFieldValidator ?? throw new ArgumentNullException(nameof(textFieldValidator));
        _textFieldValidator.PropertyName = PropertyName;
        _numberFieldValidator = numberFieldValidator ?? throw new ArgumentNullException(nameof(numberFieldValidator));
        _numberFieldValidator.PropertyName = PropertyName;
        _boolFieldValidator = boolFieldValidator ?? throw new ArgumentNullException(nameof(boolFieldValidator));
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        _boolFieldValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(CreateTemplateRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(CreateTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return StaticValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        if (value.TextFields.Count + value.NumberFields.Count + value.BoolFields.Count == 0)
        {
            PropertyName.Push("*Fields");
            return StaticValidationException(TemplateErrorMessages.NoFields);
        }

        if (_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return DynamicValidationException(TemplateErrorMessages.NameNotUnique);
        }

        for (var i = 0; i < value.TextFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.TextFields)}[{i}]");
            var textFieldValidationResult = await _textFieldValidator.ValidateAsync(value.TextFields[i]);
            PropertyName.Pop();
            if (textFieldValidationResult.Failure)
            {
                return textFieldValidationResult;
            }
        }

        for (var i = 0; i < value.NumberFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.NumberFields)}[{i}]");
            var numberFieldValidationResult = await _numberFieldValidator.ValidateAsync(value.NumberFields[i]);
            PropertyName.Pop();
            if (numberFieldValidationResult.Failure)
            {
                return numberFieldValidationResult;
            }
        }

        for (var i = 0; i < value.BoolFields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.BoolFields)}[{i}]");
            var boolFieldValidationResult = await _boolFieldValidator.ValidateAsync(value.BoolFields[i]);
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
                return DynamicValidationException(TemplateErrorMessages.FieldNameNotUnique);
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
                return DynamicValidationException(TemplateErrorMessages.FieldNameNotUnique);
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
                return DynamicValidationException(TemplateErrorMessages.FieldNameNotUnique);
            }

            allNames.Add(name);
        }

        return Result.Ok;
    }
}
