using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Templates;

public class CreateTemplateRequestValidator : Validator<CreateTemplateRequest>
{
    private readonly IValidator<BoolFieldDto> _boolFieldDtoValidator;
    private readonly IValidator<NumberFieldDto> _numberFieldDtoValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldDto> _textFieldDtoValidator;

    public CreateTemplateRequestValidator(
        IValidator<TextFieldDto> textFieldDtoValidator,
        IValidator<NumberFieldDto> numberFieldDtoValidator,
        IValidator<BoolFieldDto> boolFieldDtoValidator,
        ITemplateManager templateManager
    )
    {
        _textFieldDtoValidator =
            textFieldDtoValidator ?? throw new ArgumentNullException(nameof(textFieldDtoValidator));
        _textFieldDtoValidator.PropertyName = PropertyName;
        _numberFieldDtoValidator =
            numberFieldDtoValidator ?? throw new ArgumentNullException(nameof(numberFieldDtoValidator));
        _numberFieldDtoValidator.PropertyName = PropertyName;
        _boolFieldDtoValidator =
            boolFieldDtoValidator ?? throw new ArgumentNullException(nameof(boolFieldDtoValidator));
        _boolFieldDtoValidator.PropertyName = PropertyName;
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
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

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.Fields is null)
        {
            PropertyName.Push(nameof(CreateTemplateRequest.Fields));
            return StaticValidationException(TemplateErrorMessages.NoFields);
        }

        if (value.Fields.Count == 0)
        {
            PropertyName.Push(nameof(CreateTemplateRequest.Fields));
            return StaticValidationException(TemplateErrorMessages.NoFields);
        }

        if (_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateTemplateRequest.TemplateName));
            return DynamicValidationException(TemplateErrorMessages.NameNotUnique);
        }

        for (var i = 0; i < value.Fields.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateTemplateRequest.Fields)}[{i}]");
            switch (value.Fields[i])
            {
                case TextFieldDto textFieldValue:
                    var textFieldDtoResult = await _textFieldDtoValidator.ValidateAsync(textFieldValue);
                    if (!textFieldDtoResult.Success)
                    {
                        return textFieldDtoResult;
                    }

                    break;
                case NumberFieldDto numberFieldValue:
                    var numberFieldDtoResult = await _numberFieldDtoValidator.ValidateAsync(numberFieldValue);
                    if (!numberFieldDtoResult.Success)
                    {
                        return numberFieldDtoResult;
                    }

                    break;
                case BoolFieldDto boolFieldValue:
                    var boolFieldDtoResult = await _boolFieldDtoValidator.ValidateAsync(boolFieldValue);
                    if (!boolFieldDtoResult.Success)
                    {
                        return boolFieldDtoResult;
                    }

                    break;
                default:
                    return DynamicValidationException(TemplateErrorMessages.InvalidField);
            }

            PropertyName.Pop();
        }

        var allNames = new HashSet<string>(value.Fields.Count);

        for (var i = 0; i < value.Fields.Count; i++)
        {
            var field = value.Fields[i];
            if (allNames.Add(field.Name))
            {
                continue;
            }

            PropertyName.Push($"{nameof(CreateTemplateRequest.Fields)}[{i}]");
            PropertyName.Push(nameof(TextFieldDto.Name));
            return DynamicValidationException(TemplateErrorMessages.FieldNameNotUnique);
        }

        return Result.Ok;
    }
}
