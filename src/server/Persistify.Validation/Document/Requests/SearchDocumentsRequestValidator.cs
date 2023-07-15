using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Common;
using Persistify.Protos.Documents.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Document.Requests;

public class SearchDocumentsRequestValidator : IValidator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;
    public string ErrorPrefix { get; set; }

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator
        )
    {
        _paginationValidator = paginationValidator;
        ErrorPrefix = "SearchDocumentsRequest";
    }
    public Result Validate(SearchDocumentsRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Request is null");
        }

        if (string.IsNullOrEmpty(value.TemplateName))
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName", "TemplateName is required");
        }

        if (value.TemplateName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName",
                "TemplateName's length must be lower than or equal to 64 characters");
        }

        _paginationValidator.ErrorPrefix = $"{ErrorPrefix}.Pagination";
        var result = _paginationValidator.Validate(value.Pagination);
        if (result.IsFailure)
        {
            return result;
        }

        return Result.Success;
    }
}
