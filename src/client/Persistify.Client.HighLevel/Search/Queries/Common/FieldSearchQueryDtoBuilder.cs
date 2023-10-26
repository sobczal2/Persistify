using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Bool;

namespace Persistify.Client.HighLevel.Search.Queries.Common;

public abstract class FieldSearchQueryDtoBuilder<TDocument, TSelf> : SearchQueryDtoBuilder<TDocument>
    where TDocument : class
    where TSelf : FieldSearchQueryDtoBuilder<TDocument, TSelf>
{
    public FieldSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient) : base(
        persistifyHighLevelClient)
    {
    }

    protected abstract FieldTypeDto FieldTypeDto { get; }

    public PropertyInfo GetPropertyInfo<T>(Expression<Func<TDocument, T>> field)
    {
        if (field.Body is MemberExpression member)
        {
            if (member.Member is PropertyInfo property)
            {
                return property;
            }
            else
            {
                throw new ArgumentException("The provided expression does not target a property.", nameof(field));
            }
        }
        else if (field.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
        {
            if (memberOperand.Member is PropertyInfo property)
            {
                return property;
            }
            else
            {
                throw new ArgumentException("The provided expression does not target a property.", nameof(field));
            }
        }
        else
        {
            throw new ArgumentException("Invalid expression format.", nameof(field));
        }
    }

    public TSelf WithField<T>(Expression<Func<TDocument, T>> field)
    {
        var propertyInfo = GetPropertyInfo(field);

        var fieldAttribute =
            propertyInfo.GetCustomAttributes(typeof(PersistifyFieldAttribute), false).FirstOrDefault() as
                PersistifyFieldAttribute;

        if (fieldAttribute == null)
        {
            throw new PersistifyHighLevelClientException(
                $"Field {field} does not have {nameof(PersistifyFieldAttribute)}");
        }

        var converter =
            PersistifyHighLevelClient.GetConverter(propertyInfo.PropertyType, fieldAttribute.FieldTypeDto);

        if (converter == null)
        {
            throw new PersistifyHighLevelClientException($"Converter for field {field} is not found");
        }

        if (fieldAttribute.FieldTypeDto != FieldTypeDto)
        {
            throw new PersistifyHighLevelClientException(
                $"Field {field} is not of type {nameof(FieldTypeDto)}");
        }

        ((FieldSearchQueryDto)SearchQueryDto!).SetFieldName(fieldAttribute.Name ?? propertyInfo.Name ??
            throw new PersistifyHighLevelClientException($"Property {propertyInfo.Name} does not have name"));
        return (TSelf)this;
    }
}
