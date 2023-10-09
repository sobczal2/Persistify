using System;
using System.Threading.Tasks;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Requests.Shared;
using Persistify.Server.Indexes.Indexers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class SearchDocumentsRequestValidator : Validator<SearchDocumentsRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly ITemplateManager _templateManager;

    public SearchDocumentsRequestValidator(
        IValidator<Pagination> paginationValidator,
        ITemplateManager templateManager
    )
    {
        _paginationValidator = paginationValidator ?? throw new ArgumentNullException(nameof(paginationValidator));
        _paginationValidator.PropertyName = PropertyName;
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
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

        var template = await _templateManager.GetAsync(value.TemplateName);

        if (template is null)
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
        var searchQueryResult = await ValidateSearchQueryAsync(value.SearchQuery, template);
        PropertyName.Pop();

        if (searchQueryResult.Failure)
        {
            return searchQueryResult;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateSearchQueryAsync(SearchQuery query, Template template)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (query is null)
        {
            PropertyName.Push(nameof(SearchQuery));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (query.Boost <= 0)
        {
            PropertyName.Push(nameof(SearchQuery.Boost));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        return query switch
        {
            AndSearchQuery andSearchQuery => await ValidateAndSearchQueryAsync(andSearchQuery, template),
            OrSearchQuery orSearchQuery => await ValidateOrSearchQueryAsync(orSearchQuery, template),
            NotSearchQuery notSearchQuery => await ValidateNotSearchQueryAsync(notSearchQuery, template),
            ExactBoolSearchQuery exactBoolSearchQuery => await ValidateExactBoolSearchQueryAsync(exactBoolSearchQuery, template),
            ExactNumberSearchQuery exactNumberSearchQuery => await ValidateExactNumberSearchQueryAsync(
                exactNumberSearchQuery, template),
            GreaterNumberSearchQuery greaterNumberSearchQuery => await ValidateGreaterNumberSearchQueryAsync(
                greaterNumberSearchQuery, template),
            LessNumberSearchQuery lessNumberSearchQuery => await ValidateLessNumberSearchQueryAsync(
                lessNumberSearchQuery, template),
            RangeNumberSearchQuery rangeNumberSearchQuery => await ValidateRangeNumberSearchQueryAsync(
                rangeNumberSearchQuery, template),
            ExactTextSearchQuery exactTextSearchQuery => await ValidateExactTextSearchQueryAsync(exactTextSearchQuery, template),
            FullTextSearchQuery fullTextSearchQuery => await ValidateFullTextSearchQueryAsync(fullTextSearchQuery, template),
            PrefixTextSearchQuery prefixTextSearchQuery => await ValidatePrefixTextSearchQueryAsync(
                prefixTextSearchQuery, template),
            _ => ValidationException(SharedErrorMessages.InvalidValue)
        };
    }

    private async ValueTask<Result> ValidateAndSearchQueryAsync(AndSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(AndSearchQuery));
        if (query.Queries.Count < 2)
        {
            PropertyName.Push(nameof(AndSearchQuery.Queries));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach (var subquery in query.Queries)
        {
            PropertyName.Push(nameof(AndSearchQuery.Queries));
            var result = await ValidateSearchQueryAsync(subquery, template);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateOrSearchQueryAsync(OrSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(OrSearchQuery));
        if (query.Queries.Count < 2)
        {
            PropertyName.Push(nameof(OrSearchQuery.Queries));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach (var subquery in query.Queries)
        {
            PropertyName.Push(nameof(OrSearchQuery.Queries));
            var result = await ValidateSearchQueryAsync(subquery, template);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateNotSearchQueryAsync(NotSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(NotSearchQuery));
        PropertyName.Push(nameof(NotSearchQuery.Query));
        var result = await ValidateSearchQueryAsync(query.Query, template);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private ValueTask<Result> ValidateFieldName(string fieldName, Template template, IndexType indexType)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (fieldName.Length > 64)
        {
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        switch (indexType)
        {
            case IndexType.Text:
                if (!template.TextFieldsByName.ContainsKey(fieldName))
                {
                    return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.FieldNotFoundForThisType));
                }
                break;
            case IndexType.Number:
                if (!template.NumberFieldsByName.ContainsKey(fieldName))
                {
                    return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.FieldNotFoundForThisType));
                }
                break;
            case IndexType.Boolean:
                if (!template.BoolFieldsByName.ContainsKey(fieldName))
                {
                    return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.FieldNotFoundForThisType));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(indexType), indexType, null);
        }

        return ValueTask.FromResult(Result.Ok);
    }

    private async ValueTask<Result> ValidateExactBoolSearchQueryAsync(ExactBoolSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(ExactBoolSearchQuery));
        PropertyName.Push(nameof(ExactBoolSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Boolean);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactNumberSearchQueryAsync(ExactNumberSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(ExactNumberSearchQuery));
        PropertyName.Push(nameof(ExactNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateGreaterNumberSearchQueryAsync(GreaterNumberSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(GreaterNumberSearchQuery));
        PropertyName.Push(nameof(GreaterNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateLessNumberSearchQueryAsync(LessNumberSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(LessNumberSearchQuery));
        PropertyName.Push(nameof(LessNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateRangeNumberSearchQueryAsync(RangeNumberSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(RangeNumberSearchQuery));
        PropertyName.Push(nameof(RangeNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        if (query.MinValue > query.MaxValue)
        {
            PropertyName.Push(nameof(RangeNumberSearchQuery.MinValue));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactTextSearchQueryAsync(ExactTextSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(ExactTextSearchQuery));
        PropertyName.Push(nameof(ExactTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateFullTextSearchQueryAsync(FullTextSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(FullTextSearchQuery));
        PropertyName.Push(nameof(FullTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidatePrefixTextSearchQueryAsync(PrefixTextSearchQuery query, Template template)
    {
        PropertyName.Push(nameof(PrefixTextSearchQuery));
        PropertyName.Push(nameof(PrefixTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }
}
