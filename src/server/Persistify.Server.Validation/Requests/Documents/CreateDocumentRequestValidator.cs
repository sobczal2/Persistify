using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Documents;

public class CreateDocumentRequestValidator : Validator<CreateDocumentRequest>
{
    private readonly IValidator<BoolFieldValueDto> _boolFieldValueDtoValidator;
    private readonly IValidator<DateTimeFieldValueDto> _dateTimeFieldValueDtoValidator;
    private readonly IValidator<BinaryFieldValueDto> _binaryFieldValueDtoValidator;
    private readonly IValidator<NumberFieldValueDto> _numberFieldValueDtoValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldValueDto> _textFieldValueDtoValidator;

    public CreateDocumentRequestValidator(
        IValidator<TextFieldValueDto> textFieldValueDtoValidator,
        IValidator<NumberFieldValueDto> numberFieldValueDtoValidator,
        IValidator<BoolFieldValueDto> boolFieldValueDtoValidator,
        IValidator<DateTimeFieldValueDto> dateTimeFieldValueDtoValidator,
        IValidator<BinaryFieldValueDto> binaryFieldValueDtoValidator,
        ITemplateManager templateManager
    )
    {
        _textFieldValueDtoValidator =
            textFieldValueDtoValidator
            ?? throw new ArgumentNullException(nameof(textFieldValueDtoValidator));
        _textFieldValueDtoValidator.PropertyName = PropertyName;
        _numberFieldValueDtoValidator =
            numberFieldValueDtoValidator
            ?? throw new ArgumentNullException(nameof(numberFieldValueDtoValidator));
        _numberFieldValueDtoValidator.PropertyName = PropertyName;
        _boolFieldValueDtoValidator =
            boolFieldValueDtoValidator
            ?? throw new ArgumentNullException(nameof(boolFieldValueDtoValidator));
        _boolFieldValueDtoValidator.PropertyName = PropertyName;
        _dateTimeFieldValueDtoValidator = dateTimeFieldValueDtoValidator ??
                                          throw new ArgumentNullException(nameof(dateTimeFieldValueDtoValidator));
        _binaryFieldValueDtoValidator = binaryFieldValueDtoValidator ??
                                        throw new ArgumentNullException(nameof(binaryFieldValueDtoValidator));
        _dateTimeFieldValueDtoValidator.PropertyName = PropertyName;
        _templateManager =
            templateManager ?? throw new ArgumentNullException(nameof(templateManager));
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
        if (value.FieldValueDtos is null)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.FieldValueDtos));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.FieldValueDtos.Count == 0)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.FieldValueDtos));
            return StaticValidationException(DocumentErrorMessages.FieldValuesEmpty);
        }

        var template = await _templateManager.GetAsync(value.TemplateName);

        if (template is null)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return DynamicValidationException(DocumentErrorMessages.TemplateNotFound);
        }

        for (var i = 0; i < value.FieldValueDtos.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateDocumentRequest.FieldValueDtos)}[{i}]");
            switch (value.FieldValueDtos[i])
            {
                case TextFieldValueDto textFieldValueDto:
                    var textFieldValueDtoResult = await _textFieldValueDtoValidator.ValidateAsync(
                        textFieldValueDto
                    );
                    if (!textFieldValueDtoResult.Success)
                    {
                        return textFieldValueDtoResult;
                    }

                    break;
                case NumberFieldValueDto numberFieldValueDto:
                    var numberFieldValueDtoResult =
                        await _numberFieldValueDtoValidator.ValidateAsync(numberFieldValueDto);
                    if (!numberFieldValueDtoResult.Success)
                    {
                        return numberFieldValueDtoResult;
                    }

                    break;
                case BoolFieldValueDto boolFieldValueDto:
                    var boolFieldValueDtoResult = await _boolFieldValueDtoValidator.ValidateAsync(
                        boolFieldValueDto
                    );
                    if (!boolFieldValueDtoResult.Success)
                    {
                        return boolFieldValueDtoResult;
                    }

                    break;
                case DateTimeFieldValueDto dateTimeFieldValueDto:
                    var dateTimeFieldValueDtoResult =
                        await _dateTimeFieldValueDtoValidator.ValidateAsync(dateTimeFieldValueDto);
                    if (!dateTimeFieldValueDtoResult.Success)
                    {
                        return dateTimeFieldValueDtoResult;
                    }

                    break;
                case BinaryFieldValueDto binaryFieldValueDto:
                    var binaryFieldValueDtoResult = await _binaryFieldValueDtoValidator.ValidateAsync(
                        binaryFieldValueDto
                    );
                    if (!binaryFieldValueDtoResult.Success)
                    {
                        return binaryFieldValueDtoResult;
                    }

                    break;
                default:
                    return DynamicValidationException(DocumentErrorMessages.InvalidFieldValue);
            }

            PropertyName.Pop();
        }

        var fieldNameTypeMap = new Dictionary<string, FieldTypeDto>();

        for (var i = 0; i < value.FieldValueDtos.Count; i++)
        {
            var fieldName = value.FieldValueDtos[i].FieldName;
            if (fieldNameTypeMap.ContainsKey(fieldName))
            {
                PropertyName.Push($"{nameof(CreateDocumentRequest.FieldValueDtos)}[{i}]");
                PropertyName.Push(nameof(TextFieldValue.FieldName));
                return DynamicValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            fieldNameTypeMap.Add(fieldName, value.FieldValueDtos[i].FieldTypeDto);
        }

        foreach (var field in template.Fields)
        {
            var fieldNameType = fieldNameTypeMap.GetValueOrDefault(field.Name);
            if (fieldNameType == default)
            {
                if (field.Required)
                {
                    PropertyName.Push(nameof(CreateDocumentRequest.FieldValueDtos));
                    return DynamicValidationException(DocumentErrorMessages.RequiredFieldMissing);
                }
            }
            else
            {
                if ((int)fieldNameType != (int)field.FieldType)
                {
                    PropertyName.Push(nameof(CreateDocumentRequest.FieldValueDtos));
                    return DynamicValidationException(DocumentErrorMessages.FieldTypeMismatch);
                }
            }
        }

        return Result.Ok;
    }
}
