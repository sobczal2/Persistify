using Persistify.Domain.Documents;

namespace Persistify.Client.Objects.Converters.Number;

public class IntNumberFieldValueConverter : IFieldValueConverter<int, NumberFieldValue>
{
    public NumberFieldValue Convert(int from, string fieldName)
    {
        return new NumberFieldValue
        {
            FieldName = fieldName,
            Value = from
        };
    }

    public NumberFieldValue Convert(object? from, string fieldName)
    {
        if (from is int value)
        {
            return Convert(value, fieldName);
        }

        throw new ArgumentException($"Cannot convert {from?.GetType().Name ?? "null"} to {nameof(NumberFieldValue)}");
    }
}
