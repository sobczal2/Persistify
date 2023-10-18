using System;
using Persistify.Domain.Documents;
using Persistify.Dtos.Documents.FieldValues;

namespace Persistify.Dtos.Mappers;

public static class FieldValueMapper
{
    public static FieldValueDto Map(FieldValue from)
    {
        return from switch
        {
            BoolFieldValue boolFieldValue => Map(boolFieldValue),
            NumberFieldValue numberFieldValue => Map(numberFieldValue),
            TextFieldValue textFieldValue => Map(textFieldValue),
            _ => throw new NotSupportedException($"Field value type {from.GetType().Name} is not supported.")
        };
    }

    public static FieldValue Map(FieldValueDto from)
    {
        return from switch
        {
            BoolFieldValueDto boolFieldValueDto => Map(boolFieldValueDto),
            NumberFieldValueDto numberFieldValueDto => Map(numberFieldValueDto),
            TextFieldValueDto textFieldValueDto => Map(textFieldValueDto),
            _ => throw new NotSupportedException($"Field value type {from.GetType().Name} is not supported.")
        };
    }

    private static BoolFieldValueDto Map(BoolFieldValue from)
    {
        return new BoolFieldValueDto { FieldName = from.FieldName, Value = from.Value };
    }

    private static NumberFieldValueDto Map(NumberFieldValue from)
    {
        return new NumberFieldValueDto { FieldName = from.FieldName, Value = from.Value };
    }

    private static TextFieldValueDto Map(TextFieldValue from)
    {
        return new TextFieldValueDto { FieldName = from.FieldName, Value = from.Value };
    }

    private static BoolFieldValue Map(BoolFieldValueDto from)
    {
        return new BoolFieldValue { FieldName = from.FieldName, Value = from.Value };
    }

    private static NumberFieldValue Map(NumberFieldValueDto from)
    {
        return new NumberFieldValue { FieldName = from.FieldName, Value = from.Value };
    }

    private static TextFieldValue Map(TextFieldValueDto from)
    {
        return new TextFieldValue { FieldName = from.FieldName, Value = from.Value };
    }
}
