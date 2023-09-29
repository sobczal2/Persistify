using System;
using System.Threading.Tasks;
using Persistify.Domain.Search.Queries;
using Persistify.Requests.Documents;
using Persistify.Requests.Shared;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class SearchDocumentsRequestValidator : Validator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly IValidator<SearchQuery> _searchNodeValidator;
    private readonly ITemplateManager _templateManager;

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator,
        IValidator<SearchQuery> searchQueryValidator,
        ITemplateManager templateManager
    )
    {
        _paginationValidator = paginationValidator ?? throw new ArgumentNullException(nameof(paginationValidator));
        _paginationValidator.PropertyName = PropertyName;
        _searchNodeValidator = searchQueryValidator ?? throw new ArgumentNullException(nameof(searchQueryValidator));
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        _searchNodeValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(SearchDocumentsRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(SearchDocumentsRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return ValidationException(DocumentErrorMessages.TemplateNotFound);
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.Pagination));
        var paginationValidator = await _paginationValidator.ValidateAsync(value.Pagination);
        PropertyName.Pop();
        if (paginationValidator.Failure)
        {
            return paginationValidator;
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.SearchQuery));
        var searchNodeValidator = await _searchNodeValidator.ValidateAsync(value.SearchQuery);
        PropertyName.Pop();
        if (searchNodeValidator.Failure)
        {
            return searchNodeValidator;
        }

        return Result.Ok;
    }
}
