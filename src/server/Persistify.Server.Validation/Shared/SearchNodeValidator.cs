using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Requests.Search;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class SearchNodeValidator : Validator<SearchNode>
{
    public SearchNodeValidator()
    {
        PropertyNames.Push(nameof(SearchNode));
    }

    public override Result Validate(SearchNode value)
    {
        return ValidateSearchNode(value);
    }

    private Result ValidateSearchNode(SearchNode value)
    {
        switch (value)
        {
            case AndSearchNode andSearchNode:
                PropertyNames.Push(nameof(AndSearchNode));
                return ValidateAndSearchNode(andSearchNode);
            case OrSearchNode orSearchNode:
                PropertyNames.Push(nameof(OrSearchNode));
                return ValidateOrSearchNode(orSearchNode);
            case NotSearchNode notSearchNode:
                PropertyNames.Push(nameof(NotSearchNode));
                return ValidateNotSearchNode(notSearchNode);
            case TextSearchNode textSearchNode:
                PropertyNames.Push(nameof(TextSearchNode));
                return ValidateTextSearchNode(textSearchNode);
            case FtsSearchNode ftsSearchNode:
                PropertyNames.Push(nameof(FtsSearchNode));
                return ValidateFtsSearchNode(ftsSearchNode);
            case NumberSearchNode numberSearchNode:
                PropertyNames.Push(nameof(NumberSearchNode));
                return ValidateNumberSearchNode(numberSearchNode);
            case NumberRangeSearchNode numberRangeSearchNode:
                PropertyNames.Push(nameof(NumberRangeSearchNode));
                return ValidateNumberRangeSearchNode(numberRangeSearchNode);
            case BoolSearchNode boolSearchNode:
                PropertyNames.Push(nameof(BoolSearchNode));
                return ValidateBoolSearchNode(boolSearchNode);
            default:
                return ValidationException("Unknown SearchNode type");
        }
    }

    private Result ValidateAndSearchNode(AndSearchNode value)
    {
        if (value.Nodes.Count <= 1)
        {
            return ValidationException(DocumentErrorMessages.AndSearchNodeMustHaveAtLeastTwoNodes);
        }

        for (var i = 0; i < value.Nodes.Count; i++)
        {
            PropertyNames.Push($"{nameof(AndSearchNode.Nodes)}[{i}]");
            var result = ValidateSearchNode(value.Nodes[i]);
            PropertyNames.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private Result ValidateOrSearchNode(OrSearchNode value)
    {
        if (value.Nodes.Count <= 1)
        {
            return ValidationException(DocumentErrorMessages.OrSearchNodeMustHaveAtLeastTwoNodes);
        }

        for (var i = 0; i < value.Nodes.Count; i++)
        {
            PropertyNames.Push($"{nameof(OrSearchNode.Nodes)}[{i}]");
            var result = ValidateSearchNode(value.Nodes[i]);
            PropertyNames.Pop();
            if (result.Failure)
            {
                return result;
            }
        }

        return Result.Ok;
    }

    private Result ValidateNotSearchNode(NotSearchNode value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.Node == null)
        {
            return ValidationException(DocumentErrorMessages.NotSearchNodeMustHaveOneNode);
        }

        return ValidateSearchNode(value.Node);
    }

    private Result ValidateTextSearchNode(TextSearchNode value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(TextSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(TextSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        if (string.IsNullOrEmpty(value.Value))
        {
            PropertyNames.Push(nameof(TextSearchNode.Value));
            return ValidationException(DocumentErrorMessages.ValueEmpty);
        }

        return Result.Ok;
    }

    private Result ValidateFtsSearchNode(FtsSearchNode value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(FtsSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(FtsSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        if (string.IsNullOrEmpty(value.Value))
        {
            return ValidationException(DocumentErrorMessages.ValueEmpty);
        }

        return Result.Ok;
    }

    private Result ValidateNumberSearchNode(NumberSearchNode value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(NumberSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(NumberSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }

    private Result ValidateNumberRangeSearchNode(NumberRangeSearchNode value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(NumberRangeSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(NumberRangeSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }

    private Result ValidateBoolSearchNode(BoolSearchNode value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyNames.Push(nameof(BoolSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameEmpty);
        }

        if (value.FieldName.Length > 64)
        {
            PropertyNames.Push(nameof(BoolSearchNode.FieldName));
            return ValidationException(DocumentErrorMessages.NameTooLong);
        }

        return Result.Ok;
    }
}
