using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Documents;

public class CreateDocumentRequestValidator : Validator<CreateDocumentRequest>
{
    private readonly IValidator<BoolFieldValue> _boolFieldValueValidator;
    private readonly IValidator<NumberFieldValue> _numberFieldValueValidator;
    private readonly IValidator<TextFieldValue> _textFieldValueValidator;

    public CreateDocumentRequestValidator(
        IValidator<TextFieldValue> textFieldValueValidator,
        IValidator<NumberFieldValue> numberFieldValueValidator,
        IValidator<BoolFieldValue> boolFieldValueValidator
    )
    {
        _textFieldValueValidator = textFieldValueValidator ?? throw new ArgumentNullException(nameof(textFieldValueValidator));
        _textFieldValueValidator.PropertyNames = PropertyNames;
        _numberFieldValueValidator = numberFieldValueValidator ?? throw new ArgumentNullException(nameof(numberFieldValueValidator));
        _numberFieldValueValidator.PropertyNames = PropertyNames;
        _boolFieldValueValidator = boolFieldValueValidator ?? throw new ArgumentNullException(nameof(boolFieldValueValidator));
        _boolFieldValueValidator.PropertyNames = PropertyNames;
        PropertyNames.Push(nameof(CreateDocumentRequest));
    }

    public override Result Validate(CreateDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(CreateDocumentRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        if (value.TextFieldValues.Count == 0 && value.NumberFieldValues.Count == 0 && value.BoolFieldValues.Count == 0)
        {
            PropertyNames.Push("*FieldValues");
            return ValidationException(DocumentErrorMessages.NoFieldValues);
        }

        for (var i = 0; i < value.TextFieldValues.Count; i++)
        {
            _textFieldValueValidator.PropertyNames.Push($"{nameof(CreateDocumentRequest.TextFieldValues)}[{i}]");
            var result = _textFieldValueValidator.Validate(value.TextFieldValues[i]);
            _textFieldValueValidator.PropertyNames.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            _numberFieldValueValidator.PropertyNames.Push($"{nameof(CreateDocumentRequest.NumberFieldValues)}[{i}]");
            var result = _numberFieldValueValidator.Validate(value.NumberFieldValues[i]);
            _numberFieldValueValidator.PropertyNames.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            _boolFieldValueValidator.PropertyNames.Push($"{nameof(CreateDocumentRequest.BoolFieldValues)}[{i}]");
            var result = _boolFieldValueValidator.Validate(value.BoolFieldValues[i]);
            _boolFieldValueValidator.PropertyNames.Pop();
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
                PropertyNames.Push($"{nameof(CreateDocumentRequest.TextFieldValues)}[{i}]");
                PropertyNames.Push(nameof(TextFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            var fieldName = value.NumberFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                PropertyNames.Push($"{nameof(CreateDocumentRequest.NumberFieldValues)}[{i}]");
                PropertyNames.Push(nameof(NumberFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            var fieldName = value.BoolFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                PropertyNames.Push($"{nameof(CreateDocumentRequest.BoolFieldValues)}[{i}]");
                PropertyNames.Push(nameof(BoolFieldValue.FieldName));
                return ValidationException(DocumentErrorMessages.FieldNameNotUnique);
            }

            allFieldNames.Add(fieldName);
        }

        return Result.Ok;
    }
}
