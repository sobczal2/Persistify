using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Documents;

public class SearchDocumentsRequestValidator : IValidator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly IValidator<SearchNode> _searchNodeValidator;

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator,
        IValidator<SearchNode> searchNodeValidator
    )
    {
        _paginationValidator = paginationValidator;
        _searchNodeValidator = searchNodeValidator;
        ErrorPrefix = "SearchDocumentsRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(SearchDocumentsRequest value)
    {
        if (value.TemplateId < 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than or equal to 0");
        }

        _paginationValidator.ErrorPrefix = $"{ErrorPrefix}.Pagination";
        var paginationValidator = _paginationValidator.Validate(value.Pagination);
        if (paginationValidator.IsFailure)
        {
            return paginationValidator;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if(value.SearchNode == null)
        {
            return new ValidationException($"{ErrorPrefix}.SearchNode", "SearchNode is required");
        }

        _searchNodeValidator.ErrorPrefix = $"{ErrorPrefix}.SearchNode";
        var searchNodeValidator = _searchNodeValidator.Validate(value.SearchNode);
        if (searchNodeValidator.IsFailure)
        {
            return searchNodeValidator;
        }

        return Result.Success;
    }
}
