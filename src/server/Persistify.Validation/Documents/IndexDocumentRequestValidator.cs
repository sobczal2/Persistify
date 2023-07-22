using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Validation.Common;

namespace Persistify.Validation.Documents;

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

        return Result.Success;
    }
}
