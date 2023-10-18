using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Documents;

public class CreateDocumentRequestValidator : Validator<CreateDocumentRequest>
{
    private readonly IValidator<TextFieldValueDto> _textFieldValueDtoValidator;
    private readonly IValidator<NumberFieldValueDto> _numberFieldValueDtoValidator;
    private readonly IValidator<BoolFieldValueDto> _boolFieldValueDtoValidator;
    private readonly ITemplateManager _templateManager;

    public CreateDocumentRequestValidator(
        IValidator<TextFieldValueDto> textFieldValueDtoValidator,
        IValidator<NumberFieldValueDto> numberFieldValueDtoValidator,
        IValidator<BoolFieldValueDto> boolFieldValueDtoValidator,
        ITemplateManager templateManager
    )
    {
        _textFieldValueDtoValidator = textFieldValueDtoValidator;
        _textFieldValueDtoValidator.PropertyName = PropertyName;
        _numberFieldValueDtoValidator = numberFieldValueDtoValidator;
        _numberFieldValueDtoValidator.PropertyName = PropertyName;
        _boolFieldValueDtoValidator = boolFieldValueDtoValidator;
        _boolFieldValueDtoValidator.PropertyName = PropertyName;
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        PropertyName.Push(nameof(CreateDocumentRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(CreateDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.FieldValues is null)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.FieldValues));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.FieldValues.Count == 0)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.FieldValues));
            return DynamicValidationException(DocumentErrorMessages.FieldValuesEmpty);
        }

        var template = await _templateManager.GetAsync(value.TemplateName);

        if (template is null)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return DynamicValidationException(DocumentErrorMessages.TemplateNotFound);
        }


        for (var i = 0; i < value.FieldValues.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateDocumentRequest.FieldValues)}[{i}]");
            switch (value.FieldValues[i])
            {
                case TextFieldValueDto textFieldValue:
                    var textFieldValueDtoResult = await _textFieldValueDtoValidator.ValidateAsync(textFieldValue);
                    if (!textFieldValueDtoResult.Success)
                    {
                        return textFieldValueDtoResult;
                    }

                    break;
                case NumberFieldValueDto numberFieldValue:
                    var numberFieldValueDtoResult = await _numberFieldValueDtoValidator.ValidateAsync(numberFieldValue);
                    if (!numberFieldValueDtoResult.Success)
                    {
                        return numberFieldValueDtoResult;
                    }

                    break;
                case BoolFieldValueDto boolFieldValue:
                    var boolFieldValueDtoResult = await _boolFieldValueDtoValidator.ValidateAsync(boolFieldValue);
                    if (!boolFieldValueDtoResult.Success)
                    {
                        return boolFieldValueDtoResult;
                    }

                    break;
                default:
                    return DynamicValidationException(DocumentErrorMessages.InvalidFieldValue);
            }
            PropertyName.Pop();
        }

        var fieldNameTypeMap = new Dictionary<string, FieldType>();

        for (var i = 0; i < value.FieldValues.Count; i++)
        {
            var fieldName = value.FieldValues[i].FieldName;
            if (fieldNameTypeMap.ContainsKey(fieldName))
            {
                PropertyName.Push($"{nameof(CreateDocumentRequest.FieldValues)}[{i}]");
                PropertyName.Push(nameof(TextFieldValue.FieldName));
                return DynamicValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            fieldNameTypeMap.Add(fieldName, value.FieldValues[i].FieldType);
        }

        foreach (var field in template.Fields)
        {
            var fieldNameType = fieldNameTypeMap.GetValueOrDefault(field.Name);
            if (fieldNameType == default)
            {
                if (field.Required)
                {
                    PropertyName.Push(nameof(CreateDocumentRequest.FieldValues));
                    return DynamicValidationException(DocumentErrorMessages.RequiredFieldMissing);
                }
            }
            else
            {
                if (fieldNameType != field.FieldType)
                {
                    PropertyName.Push(nameof(CreateDocumentRequest.FieldValues));
                    return DynamicValidationException(DocumentErrorMessages.FieldTypeMismatch);
                }
            }
        }

        return Result.Ok;
    }
}
