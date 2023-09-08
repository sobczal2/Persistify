using Persistify.Requests.Search;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class SearchNodeValidator : IValidator<SearchNode>
{
    public SearchNodeValidator()
    {
        ErrorPrefix = "SearchNode";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(SearchNode value)
    {
        return ValidateSearchNode(value, ErrorPrefix);
    }

    private Result ValidateSearchNode(SearchNode value, string errorPrefix)
    {
        switch (value)
        {
            case AndSearchNode andSearchNode:
                return ValidateAndSearchNode(andSearchNode, errorPrefix);
            case OrSearchNode orSearchNode:
                return ValidateOrSearchNode(orSearchNode, errorPrefix);
            case NotSearchNode notSearchNode:
                return ValidateNotSearchNode(notSearchNode, errorPrefix);
            case TextSearchNode textSearchNode:
                return ValidateTextSearchNode(textSearchNode, errorPrefix);
            case FtsSearchNode ftsSearchNode:
                return ValidateFtsSearchNode(ftsSearchNode, errorPrefix);
            case NumberSearchNode numberSearchNode:
                return ValidateNumberSearchNode(numberSearchNode, errorPrefix);
            case NumberRangeSearchNode numberRangeSearchNode:
                return ValidateNumberRangeSearchNode(numberRangeSearchNode, errorPrefix);
            case BoolSearchNode boolSearchNode:
                return ValidateBoolSearchNode(boolSearchNode, errorPrefix);
            default:
                return new ValidationException(ErrorPrefix, "Unknown SearchNode type");
        }
    }

    private Result ValidateAndSearchNode(AndSearchNode value, string errorPrefix)
    {
        if (value.Nodes.Count <= 1)
        {
            return new ValidationException(ErrorPrefix, "AndSearchNode must have at least 2 nodes");
        }

        for (var i = 0; i < value.Nodes.Count; i++)
        {
            var result = ValidateSearchNode(value.Nodes[i], $"{errorPrefix}.Nodes[{i}]");
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private Result ValidateOrSearchNode(OrSearchNode value, string errorPrefix)
    {
        if (value.Nodes.Count <= 1)
        {
            return new ValidationException(ErrorPrefix, "OrSearchNode must have at least 2 nodes");
        }

        for (var i = 0; i < value.Nodes.Count; i++)
        {
            var result = ValidateSearchNode(value.Nodes[i], $"{errorPrefix}.Nodes[{i}]");
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private Result ValidateNotSearchNode(NotSearchNode value, string errorPrefix)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.Node == null)
        {
            return new ValidationException(ErrorPrefix, "NotSearchNode must have exactly 1 node");
        }

        return ValidateSearchNode(value.Node, $"{errorPrefix}.Node");
    }

    private Result ValidateTextSearchNode(TextSearchNode value, string errorPrefix)
    {
        var fieldNameResult = ValidateFieldName(value.FieldName, errorPrefix);
        if (fieldNameResult.Failure)
        {
            return fieldNameResult;
        }


        if (string.IsNullOrEmpty(value.Value))
        {
            return new ValidationException($"{errorPrefix}.Value", "Value must not be empty");
        }

        return Result.Ok;
    }

    private Result ValidateFtsSearchNode(FtsSearchNode value, string errorPrefix)
    {
        var fieldNameResult = ValidateFieldName(value.FieldName, errorPrefix);
        if (fieldNameResult.Failure)
        {
            return fieldNameResult;
        }

        if (string.IsNullOrEmpty(value.Value))
        {
            return new ValidationException($"{errorPrefix}.Value", "Value must not be empty");
        }

        return Result.Ok;
    }

    private Result ValidateNumberSearchNode(NumberSearchNode value, string errorPrefix)
    {
        var fieldNameResult = ValidateFieldName(value.FieldName, errorPrefix);
        if (fieldNameResult.Failure)
        {
            return fieldNameResult;
        }

        return Result.Ok;
    }

    private Result ValidateNumberRangeSearchNode(NumberRangeSearchNode value, string errorPrefix)
    {
        var fieldNameResult = ValidateFieldName(value.FieldName, errorPrefix);
        if (fieldNameResult.Failure)
        {
            return fieldNameResult;
        }

        return Result.Ok;
    }

    private Result ValidateBoolSearchNode(BoolSearchNode value, string errorPrefix)
    {
        var fieldNameResult = ValidateFieldName(value.FieldName, errorPrefix);
        if (fieldNameResult.Failure)
        {
            return fieldNameResult;
        }

        return Result.Ok;
    }

    private Result ValidateFieldName(string fieldName, string errorPrefix)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            return new ValidationException($"{errorPrefix}.FieldName", "FieldName must not be empty");
        }

        if (fieldName.Length > 64)
        {
            return new ValidationException($"{errorPrefix}.FieldName",
                "FieldName must not be longer than 64 characters");
        }

        return Result.Ok;
    }
}
