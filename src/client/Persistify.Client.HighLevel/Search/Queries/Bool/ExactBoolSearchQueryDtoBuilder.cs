using System;
using System.Linq;
using System.Linq.Expressions;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Helpers.Results;

namespace Persistify.Client.HighLevel.Search.Queries.Bool;

public class ExactBoolSearchQueryDtoBuilder<TDocument> : SearchQueryDtoBuilder<TDocument>
    where TDocument : class
{
    public ExactBoolSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new ExactBoolSearchQueryDto { Boost = 1 };
    }

    public ExactBoolSearchQueryDtoBuilder<TDocument> WithValue(bool value)
    {
        ((ExactBoolSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }

    public ExactBoolSearchQueryDtoBuilder<TDocument> WithField(Expression<Func<TDocument, object>> field)
    {
        if (!(field.Body is UnaryExpression unaryExpression))
        {
            throw new PersistifyHighLevelClientException($"Expression {field} is not a unary expression");
        }

        if (!(unaryExpression.Operand is MemberExpression memberExpression))
        {
            throw new PersistifyHighLevelClientException($"Expression {field} is not a member expression");
        }

        var propertyInfo = typeof(TDocument).GetProperty(memberExpression.Member.Name);

        if (propertyInfo is null)
        {
            throw new PersistifyHighLevelClientException(
                $"Property {memberExpression.Member.Name} not found on type {typeof(TDocument).Name}");
        }

        var fieldAttribute =
            propertyInfo.GetCustomAttributes(typeof(PersistifyFieldAttribute), false).FirstOrDefault() as
                PersistifyFieldAttribute;

        if (fieldAttribute == null)
        {
            throw new PersistifyHighLevelClientException(
                $"Field {field} does not have {nameof(PersistifyFieldAttribute)}");
        }

        var converter =
            PersistifyHighLevelClient.GetConverter(unaryExpression.Operand.Type, fieldAttribute.FieldTypeDto);

        if (converter == null)
        {
            throw new PersistifyHighLevelClientException($"Converter for field {field} is not found");
        }

        ((ExactBoolSearchQueryDto)SearchQueryDto!).FieldName = fieldAttribute.Name ?? propertyInfo.Name ?? throw new PersistifyHighLevelClientException($"Property {propertyInfo.Name} does not have name");
        return this;
    }

    public ExactBoolSearchQueryDtoBuilder<TDocument> WithBoost(float boost)
    {
        ((ExactBoolSearchQueryDto)SearchQueryDto!).Boost = boost;
        return this;
    }
}
