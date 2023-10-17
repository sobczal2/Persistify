using System;
using Persistify.Domain.Documents;

namespace Persistify.Client.Objects.Converters.Number;

public class DoubleNumberFieldValueConverter : IFieldValueConverter<double, NumberFieldValue>
{
    public NumberFieldValue Convert(double from, string fieldName)
    {
        return new NumberFieldValue
        {
            FieldName = fieldName,
            Value = from
        };
    }

    public NumberFieldValue Convert(object? from, string fieldName)
    {
        if (from is double value)
        {
            return Convert(value, fieldName);
        }

        throw new ArgumentException($"Cannot convert {from?.GetType().Name ?? "null"} to {nameof(NumberFieldValue)}");
    }
}
