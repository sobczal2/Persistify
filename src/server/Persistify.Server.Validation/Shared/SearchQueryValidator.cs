using System;
using System.Threading.Tasks;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class SearchQueryValidator : Validator<SearchQuery>
{
    public override async ValueTask<Result> ValidateNotNullAsync(SearchQuery value)
    {
        if(value.Boost <= 0)
        {
            PropertyName.Push(nameof(SearchQuery.Boost));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        return value switch
        {
            AndSearchQuery andSearchQuery => await ValidateAndSearchQueryAsync(andSearchQuery),
            OrSearchQuery orSearchQuery => await ValidateOrSearchQueryAsync(orSearchQuery),
            NotSearchQuery notSearchQuery => await ValidateNotSearchQueryAsync(notSearchQuery),
            ExactBoolSearchQuery exactBoolSearchQuery => await ValidateExactBoolSearchQueryAsync(exactBoolSearchQuery),
            ExactNumberSearchQuery exactNumberSearchQuery => await ValidateExactNumberSearchQueryAsync(
                exactNumberSearchQuery),
            GreaterNumberSearchQuery greaterNumberSearchQuery => await ValidateGreaterNumberSearchQueryAsync(
                greaterNumberSearchQuery),
            LessNumberSearchQuery lessNumberSearchQuery => await ValidateLessNumberSearchQueryAsync(
                lessNumberSearchQuery),
            RangeNumberSearchQuery rangeNumberSearchQuery => await ValidateRangeNumberSearchQueryAsync(
                rangeNumberSearchQuery),
            ExactTextSearchQuery exactTextSearchQuery => await ValidateExactTextSearchQueryAsync(exactTextSearchQuery),
            FullTextSearchQuery fullTextSearchQuery => await ValidateFullTextSearchQueryAsync(fullTextSearchQuery),
            PrefixTextSearchQuery prefixTextSearchQuery => await ValidatePrefixTextSearchQueryAsync(
                prefixTextSearchQuery),
            _ => ValidationException(SharedErrorMessages.InvalidValue)
        };
    }

    private async ValueTask<Result> ValidateAndSearchQueryAsync(AndSearchQuery value)
    {
        if(value.Queries.Count < 2)
        {
            PropertyName.Push(nameof(AndSearchQuery.Queries));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach(var query in value.Queries)
        {
            PropertyName.Push(nameof(AndSearchQuery.Queries));
            var result = await ValidateAsync(query);
            PropertyName.Pop();
            if(result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateOrSearchQueryAsync(OrSearchQuery value)
    {
        if(value.Queries.Count < 2)
        {
            PropertyName.Push(nameof(OrSearchQuery.Queries));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        foreach(var query in value.Queries)
        {
            PropertyName.Push(nameof(OrSearchQuery.Queries));
            var result = await ValidateAsync(query);
            PropertyName.Pop();
            if(result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateNotSearchQueryAsync(NotSearchQuery value)
    {
        PropertyName.Push(nameof(NotSearchQuery.Query));
        var result = await ValidateAsync(value.Query);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private ValueTask<Result> ValidateFieldName(string fieldName)
    {
        if(string.IsNullOrEmpty(fieldName))
        {
            PropertyName.Push("FieldName");
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if(fieldName.Length > 64)
        {
            PropertyName.Push("FieldName");
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }

    private async ValueTask<Result> ValidateExactBoolSearchQueryAsync(ExactBoolSearchQuery query)
    {
        PropertyName.Push(nameof(ExactBoolSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactNumberSearchQueryAsync(ExactNumberSearchQuery query)
    {
        PropertyName.Push(nameof(ExactNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateGreaterNumberSearchQueryAsync(GreaterNumberSearchQuery query)
    {
        PropertyName.Push(nameof(GreaterNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateLessNumberSearchQueryAsync(LessNumberSearchQuery query)
    {
        PropertyName.Push(nameof(LessNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateRangeNumberSearchQueryAsync(RangeNumberSearchQuery query)
    {
        PropertyName.Push(nameof(RangeNumberSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        if(query.MinValue > query.MaxValue)
        {
            PropertyName.Push(nameof(RangeNumberSearchQuery.MinValue));
            return ValidationException(SharedErrorMessages.InvalidValue);
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateExactTextSearchQueryAsync(ExactTextSearchQuery query)
    {
        PropertyName.Push(nameof(ExactTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidateFullTextSearchQueryAsync(FullTextSearchQuery query)
    {
        PropertyName.Push(nameof(FullTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }

    private async ValueTask<Result> ValidatePrefixTextSearchQueryAsync(PrefixTextSearchQuery query)
    {
        PropertyName.Push(nameof(PrefixTextSearchQuery.FieldName));
        var result = await ValidateFieldName(query.FieldName);
        PropertyName.Pop();
        if(result.Failure)
        {
            return result;
        }

        return Result.Ok;
    }
}
