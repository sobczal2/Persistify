using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Documents;

public class SearchDocumentsRequestValidator : IValidator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator
    )
    {
        _paginationValidator = paginationValidator;
        ErrorPrefix = "SearchDocumentsRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(SearchDocumentsRequest value)
    {
        if (value.TemplateId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than 0");
        }

        _paginationValidator.ErrorPrefix = $"{ErrorPrefix}.Pagination";
        var result = _paginationValidator.Validate(value.Pagination);
        if (!result.IsSuccess)
        {
            return result;
        }

        return Result.Success;
    }
}
