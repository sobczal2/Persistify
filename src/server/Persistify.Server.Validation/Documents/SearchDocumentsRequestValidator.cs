using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Documents;

public class SearchDocumentsRequestValidator : Validator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly IValidator<SearchNode> _searchNodeValidator;

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator,
        IValidator<SearchNode> searchNodeValidator
    )
    {
        _paginationValidator = paginationValidator;
        _paginationValidator.PropertyNames = PropertyNames;
        _searchNodeValidator = searchNodeValidator;
        _searchNodeValidator.PropertyNames = PropertyNames;
    }

    public override Result Validate(SearchDocumentsRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(SearchDocumentsRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        PropertyNames.Push(nameof(SearchDocumentsRequest.Pagination));
        var paginationValidator = _paginationValidator.Validate(value.Pagination);
        PropertyNames.Pop();
        if (paginationValidator.Failure)
        {
            return paginationValidator;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.SearchNode == null)
        {
            return ValidationException(DocumentErrorMessages.SearchNodeNull);
        }

        PropertyNames.Push(nameof(SearchDocumentsRequest.SearchNode));
        var searchNodeValidator = _searchNodeValidator.Validate(value.SearchNode);
        PropertyNames.Pop();
        if (searchNodeValidator.Failure)
        {
            return searchNodeValidator;
        }

        return Result.Ok;
    }
}
