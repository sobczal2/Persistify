using System;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

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
        _paginationValidator = paginationValidator ?? throw new ArgumentNullException(nameof(paginationValidator));
        _paginationValidator.PropertyName = PropertyName;
        _searchNodeValidator = searchNodeValidator ?? throw new ArgumentNullException(nameof(searchNodeValidator));
        _searchNodeValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(SearchDocumentsRequest));
    }

    public override Result ValidateNotNull(SearchDocumentsRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.Pagination == null)
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.Pagination));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.Pagination));
        var paginationValidator = _paginationValidator.Validate(value.Pagination);
        PropertyName.Pop();
        if (paginationValidator.Failure)
        {
            return paginationValidator;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.SearchNode == null)
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.SearchNode));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.SearchNode));
        var searchNodeValidator = _searchNodeValidator.Validate(value.SearchNode);
        PropertyName.Pop();
        if (searchNodeValidator.Failure)
        {
            return searchNodeValidator;
        }

        return Result.Ok;
    }
}
