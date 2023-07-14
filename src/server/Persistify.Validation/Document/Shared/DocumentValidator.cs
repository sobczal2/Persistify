using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Documents.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Document.Shared;

public class DocumentValidator : IValidator<Protos.Documents.Shared.Document>
{
    private readonly IValidator<BoolField> _boolFieldValidator;
    private readonly IValidator<NumberField> _numberFieldValidator;
    private readonly IValidator<TextField> _textFieldValidator;

    public DocumentValidator(
        IValidator<TextField> textFieldValidator,
        IValidator<NumberField> numberFieldValidator,
        IValidator<BoolField> boolFieldValidator
    )
    {
        _textFieldValidator = textFieldValidator;
        _numberFieldValidator = numberFieldValidator;
        _boolFieldValidator = boolFieldValidator;
        ErrorPrefix = "Document";
    }

    public string ErrorPrefix { get; set; }

    public Result<Unit> Validate(Protos.Documents.Shared.Document value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Document is null");
        }

        for (var i = 0; i < value.TextFields.Length; i++)
        {
            var textField = value.TextFields[i];

            _textFieldValidator.ErrorPrefix = $"{ErrorPrefix}.TextFields[{i}]";

            var result = _textFieldValidator.Validate(textField);
            if (result.IsFailure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.NumberFields.Length; i++)
        {
            var numberField = value.NumberFields[i];

            _numberFieldValidator.ErrorPrefix = $"{ErrorPrefix}.NumberFields[{i}]";

            var result = _numberFieldValidator.Validate(numberField);
            if (result.IsFailure)
            {
                return result;
            }
        }

        for (var i = 0; i < value.BoolFields.Length; i++)
        {
            var boolField = value.BoolFields[i];

            _boolFieldValidator.ErrorPrefix = $"{ErrorPrefix}.BoolFields[{i}]";

            var result = _boolFieldValidator.Validate(boolField);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return new Result<Unit>(Unit.Value);
    }
}
