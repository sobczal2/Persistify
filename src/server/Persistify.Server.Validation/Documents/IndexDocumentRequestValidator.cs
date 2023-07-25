using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Documents;

public class IndexDocumentRequestValidator : IValidator<IndexDocumentRequest>
{
    private readonly IValidator<BoolFieldValue> _boolFieldValueValidator;
    private readonly IValidator<NumberFieldValue> _numberFieldValueValidator;
    private readonly IValidator<TextFieldValue> _textFieldValueValidator;

    public IndexDocumentRequestValidator(
        IValidator<TextFieldValue> textFieldValueValidator,
        IValidator<NumberFieldValue> numberFieldValueValidator,
        IValidator<BoolFieldValue> boolFieldValueValidator
    )
    {
        _textFieldValueValidator = textFieldValueValidator;
        _numberFieldValueValidator = numberFieldValueValidator;
        _boolFieldValueValidator = boolFieldValueValidator;
        ErrorPrefix = "IndexDocumentRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(IndexDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than 0");
        }

        if(value.TextFieldValues.Count == 0 && value.NumberFieldValues.Count == 0 && value.BoolFieldValues.Count == 0)
        {
            return new ValidationException($"{ErrorPrefix}", "At least one field value is required");
        }

        for (var i = 0; i < value.TextFieldValues.Count; i++)
        {
            _textFieldValueValidator.ErrorPrefix = $"{ErrorPrefix}.TextFieldValues[{i}]";
            var result = _textFieldValueValidator.Validate(value.TextFieldValues[i]);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            _numberFieldValueValidator.ErrorPrefix = $"{ErrorPrefix}.NumberFieldValues[{i}]";
            var result = _numberFieldValueValidator.Validate(value.NumberFieldValues[i]);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            _boolFieldValueValidator.ErrorPrefix = $"{ErrorPrefix}.BoolFieldValues[{i}]";
            var result = _boolFieldValueValidator.Validate(value.BoolFieldValues[i]);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        var allFieldNames = new HashSet<string>(value.TextFieldValues.Count + value.NumberFieldValues.Count + value.BoolFieldValues.Count);

        for (var i = 0; i < value.TextFieldValues.Count; i++)
        {
            var fieldName = value.TextFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                return new ValidationException($"{ErrorPrefix}.TextFieldValues[{i}].FieldName", $"Field name '{fieldName}' is not unique");
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.NumberFieldValues.Count; i++)
        {
            var fieldName = value.NumberFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                return new ValidationException($"{ErrorPrefix}.NumberFieldValues[{i}].FieldName", $"Field name '{fieldName}' is not unique");
            }

            allFieldNames.Add(fieldName);
        }

        for (var i = 0; i < value.BoolFieldValues.Count; i++)
        {
            var fieldName = value.BoolFieldValues[i].FieldName;
            if (allFieldNames.Contains(fieldName))
            {
                return new ValidationException($"{ErrorPrefix}.BoolFieldValues[{i}].FieldName", $"Field name '{fieldName}' is not unique");
            }

            allFieldNames.Add(fieldName);
        }

        return Result.Success;
    }
}
