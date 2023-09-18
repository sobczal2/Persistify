using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class CreateDocumentRequestValidator : Validator<CreateDocumentRequest>
{
    private readonly IValidator<BoolFieldValue> _boolFieldValueValidator;
    private readonly IValidator<NumberFieldValue> _numberFieldValueValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldValue> _textFieldValueValidator;

    public CreateDocumentRequestValidator(
        IValidator<TextFieldValue> textFieldValueValidator,
        IValidator<NumberFieldValue> numberFieldValueValidator,
        IValidator<BoolFieldValue> boolFieldValueValidator,
        ITemplateManager templateManager
    )
    {
        _textFieldValueValidator =
            textFieldValueValidator ?? throw new ArgumentNullException(nameof(textFieldValueValidator));
        _textFieldValueValidator.PropertyName = PropertyName;
        _numberFieldValueValidator = numberFieldValueValidator ??
                                     throw new ArgumentNullException(nameof(numberFieldValueValidator));
        _numberFieldValueValidator.PropertyName = PropertyName;
        _boolFieldValueValidator =
            boolFieldValueValidator ?? throw new ArgumentNullException(nameof(boolFieldValueValidator));
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        _boolFieldValueValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(CreateDocumentRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(CreateDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(CreateDocumentRequest.TemplateName));
            return ValidationException(DocumentErrorMessages.TemplateNotFound);
        }

        if (value.TextFieldValues.Count + value.NumberFieldValues.Count + value.BoolFieldValues.Count == 0)
        {
            PropertyName.Push("*FieldValues");
            return ValidationException(DocumentErrorMessages.NoFieldValues);
        }

        for (var i = 0; i < value.TextFieldValues.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateDocumentRequest.TextFieldValues)}[{i}]");
            var result = await _textFieldValueValidator.ValidateAsync(value.TextFieldValues[i]);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateDocumentRequest.NumberFieldValues)}[{i}]");
            var result = await _numberFieldValueValidator.ValidateAsync(value.NumberFieldValues[i]);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            PropertyName.Push($"{nameof(CreateDocumentRequest.BoolFieldValues)}[{i}]");
            var result = await _boolFieldValueValidator.ValidateAsync(value.BoolFieldValues[i]);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        var allFieldNames = new HashSet<string>(value.TextFieldValues.Count + value.NumberFieldValues.Count +
                                                value.BoolFieldValues.Count);

        for (var i = 0; i < value.TextFieldValues.Count; i++)
        {
            var fieldName = value.TextFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                PropertyName.Push($"{nameof(CreateDocumentRequest.TextFieldValues)}[{i}]");
                PropertyName.Push(nameof(TextFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            var fieldName = value.NumberFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                PropertyName.Push($"{nameof(CreateDocumentRequest.NumberFieldValues)}[{i}]");
                PropertyName.Push(nameof(NumberFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            var fieldName = value.BoolFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                PropertyName.Push($"{nameof(CreateDocumentRequest.BoolFieldValues)}[{i}]");
                PropertyName.Push(nameof(BoolFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        return Result.Ok;
    }
}
