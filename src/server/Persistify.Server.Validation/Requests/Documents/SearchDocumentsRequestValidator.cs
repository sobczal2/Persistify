using System;
using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Dtos.Documents.Search.Queries.Number;
using Persistify.Dtos.Documents.Search.Queries.Text;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.Domain.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Documents;

public class SearchDocumentsRequestValidator : Validator<SearchDocumentsRequest>
{
    private readonly IValidator<PaginationDto> _paginationValidator;
    private readonly ITemplateManager _templateManager;

    public SearchDocumentsRequestValidator(
        IValidator<PaginationDto> paginationValidator,
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
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        var template = await _templateManager.GetAsync(value.TemplateName);

        if (template is null)
        {
            PropertyName.Push(nameof(SearchDocumentsRequest.TemplateName));
            return DynamicValidationException(DocumentErrorMessages.TemplateNotFound);
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.PaginationDto));
        var paginationValidator = await _paginationValidator.ValidateAsync(value.PaginationDto);
        PropertyName.Pop();

        if (paginationValidator.Failure)
        {
            return paginationValidator;
        }

        PropertyName.Push(nameof(SearchDocumentsRequest.SearchQueryDto));
        var searchQueryResult = await ValidateSearchQueryAsync(value.SearchQueryDto, template);
        PropertyName.Pop();

        if (searchQueryResult.Failure)
        {
            return searchQueryResult;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateSearchQueryAsync(SearchQueryDto queryDto, Template template)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (queryDto is null)
        {
            PropertyName.Push(nameof(SearchQueryDto));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (queryDto.Boost <= 0)
        {
            PropertyName.Push(nameof(SearchQueryDto.Boost));
            return StaticValidationException(SharedErrorMessages.InvalidValue);
        }

        return queryDto switch
        {
            AndSearchQueryDto andSearchQueryDto => await ValidateAndSearchQueryDtoAsync(andSearchQueryDto, template),
            OrSearchQueryDto orSearchQueryDto => await ValidateOrSearchQueryDtoAsync(orSearchQueryDto, template),
            NotSearchQueryDto notSearchQueryDto => await ValidateNotSearchQueryDtoAsync(notSearchQueryDto, template),
            ExactBoolSearchQueryDto exactBoolSearchQueryDto => await ValidateExactBoolSearchQueryDtoAsync(
                exactBoolSearchQueryDto,
                template),
            ExactNumberSearchQueryDto exactNumberSearchQueryDto => await ValidateExactNumberSearchQueryDtoAsync(
                exactNumberSearchQueryDto, template),
            GreaterNumberSearchQueryDto greaterNumberSearchQueryDto => await ValidateGreaterNumberSearchQueryDtoAsync(
                greaterNumberSearchQueryDto, template),
            LessNumberSearchQueryDto lessNumberSearchQueryDto => await ValidateLessNumberSearchQueryDtoAsync(
                lessNumberSearchQueryDto, template),
            RangeNumberSearchQueryDto rangeNumberSearchQueryDto => await ValidateRangeNumberSearchQueryDtoAsync(
                rangeNumberSearchQueryDto, template),
            ExactTextSearchQueryDto exactTextSearchQueryDto => await ValidateExactTextSearchQueryDtoAsync(
                exactTextSearchQueryDto,
                template),
            FullTextSearchQueryDto fullTextSearchQueryDto => await ValidateFullTextSearchQueryDtoAsync(
                fullTextSearchQueryDto,
                template),
            PrefixTextSearchQueryDto prefixTextSearchQueryDto => await ValidatePrefixTextSearchQueryDtoAsync(
                prefixTextSearchQueryDto, template),
            _ => StaticValidationException(SharedErrorMessages.InvalidValue)
        };
    }

    private async ValueTask<Result> ValidateAndSearchQueryDtoAsync(AndSearchQueryDto queryDto, Template template)
    {
        PropertyName.Push(nameof(AndSearchQueryDto));
        if (queryDto.SearchQueryDtos.Count < 2)
        {
            PropertyName.Push(nameof(AndSearchQueryDto.SearchQueryDtos));
            return StaticValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach (var subquery in queryDto.SearchQueryDtos)
        {
            PropertyName.Push(nameof(AndSearchQueryDto.SearchQueryDtos));
            var result = await ValidateSearchQueryAsync(subquery, template);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateOrSearchQueryDtoAsync(OrSearchQueryDto queryDto, Template template)
    {
        PropertyName.Push(nameof(OrSearchQueryDto));
        if (queryDto.SearchQueryDtos.Count < 2)
        {
            PropertyName.Push(nameof(OrSearchQueryDto.SearchQueryDtos));
            return StaticValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach (var subquery in queryDto.SearchQueryDtos)
        {
            PropertyName.Push(nameof(OrSearchQueryDto.SearchQueryDtos));
            var result = await ValidateSearchQueryAsync(subquery, template);
            PropertyName.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateNotSearchQueryDtoAsync(NotSearchQueryDto queryDto, Template template)
    {
        PropertyName.Push(nameof(NotSearchQueryDto));
        PropertyName.Push(nameof(NotSearchQueryDto.QueryDto));
        var result = await ValidateSearchQueryAsync(queryDto.QueryDto, template);
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
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (fieldName.Length > 64)
        {
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        var field = template.GetFieldByName(fieldName);

        if (field is null)
        {
            return ValueTask.FromResult<Result>(DynamicValidationException(DocumentErrorMessages.FieldNotFound));
        }

        switch (indexType)
        {
            case IndexType.Boolean when field.FieldType != FieldType.Bool:
                return ValueTask.FromResult<Result>(
                    DynamicValidationException(DocumentErrorMessages.FieldTypeMismatch));
            case IndexType.Number when field.FieldType != FieldType.Number:
                return ValueTask.FromResult<Result>(
                    DynamicValidationException(DocumentErrorMessages.FieldTypeMismatch));
            case IndexType.Text when field.FieldType != FieldType.Text:
                return ValueTask.FromResult<Result>(
                    DynamicValidationException(DocumentErrorMessages.FieldTypeMismatch));
        }

        return ValueTask.FromResult(Result.Ok);
    }

    private async ValueTask<Result> ValidateExactBoolSearchQueryDtoAsync(ExactBoolSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(ExactBoolSearchQueryDto));
        PropertyName.Push(nameof(ExactBoolSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Boolean);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactNumberSearchQueryDtoAsync(ExactNumberSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(ExactNumberSearchQueryDto));
        PropertyName.Push(nameof(ExactNumberSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateGreaterNumberSearchQueryDtoAsync(GreaterNumberSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(GreaterNumberSearchQueryDto));
        PropertyName.Push(nameof(GreaterNumberSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateLessNumberSearchQueryDtoAsync(LessNumberSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(LessNumberSearchQueryDto));
        PropertyName.Push(nameof(LessNumberSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateRangeNumberSearchQueryDtoAsync(RangeNumberSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(RangeNumberSearchQueryDto));
        PropertyName.Push(nameof(RangeNumberSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Number);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        if (queryDto.MinValue > queryDto.MaxValue)
        {
            PropertyName.Push(nameof(RangeNumberSearchQueryDto.MinValue));
            return StaticValidationException(SharedErrorMessages.InvalidValue);
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactTextSearchQueryDtoAsync(ExactTextSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(ExactTextSearchQueryDto));
        PropertyName.Push(nameof(ExactTextSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateFullTextSearchQueryDtoAsync(FullTextSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(FullTextSearchQueryDto));
        PropertyName.Push(nameof(FullTextSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidatePrefixTextSearchQueryDtoAsync(PrefixTextSearchQueryDto queryDto,
        Template template)
    {
        PropertyName.Push(nameof(PrefixTextSearchQueryDto));
        PropertyName.Push(nameof(PrefixTextSearchQueryDto.FieldName));
        var result = await ValidateFieldName(queryDto.FieldName, template, IndexType.Text);
        PropertyName.Pop();
        if (result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }
}
