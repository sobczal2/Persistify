using System;
using Persistify.Domain.Documents;

namespace Persistify.Client.Objects.Converters.Text;

public class StringTextFieldValueConverter : IFieldValueConverter<string, TextFieldValue>
{
    public TextFieldValue Convert(string from, string fieldName)
    {
        return new TextFieldValue
        {
            FieldName = fieldName,
            Value = from
        };
    }

    public TextFieldValue Convert(object? from, string fieldName)
    {
        if (from is string value)
        {
            return Convert(value, fieldName);
        }

        throw new ArgumentException($"Cannot convert {from?.GetType().Name ?? "null"} to {nameof(TextFieldValue)}");
    }
}
