using System;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.Templates;
using Persistify.Dtos.Templates.Fields;

namespace Persistify.Dtos.Mappers;

public static class FieldMapper
{
    public static FieldDto Map(Field from)
    {
        return from switch
        {
            BoolField boolFieldValue => Map(boolFieldValue),
            NumberField numberFieldValue => Map(numberFieldValue),
            TextField textFieldValue => Map(textFieldValue),
            _ => throw new NotSupportedException($"Field value type {from.GetType().Name} is not supported.")
        };
    }

    public static Field Map(FieldDto from)
    {
        return from switch
        {
            BoolFieldDto boolFieldValueDto => Map(boolFieldValueDto),
            NumberFieldDto numberFieldValueDto => Map(numberFieldValueDto),
            TextFieldDto textFieldValueDto => Map(textFieldValueDto),
            _ => throw new NotSupportedException($"Field value type {from.GetType().Name} is not supported.")
        };
    }

    private static BoolFieldDto Map(BoolField from)
    {
        return new BoolFieldDto { Name = from.Name, Required = from.Required };
    }

    private static NumberFieldDto Map(NumberField from)
    {
        return new NumberFieldDto { Name = from.Name, Required = from.Required };
    }

    private static TextFieldDto Map(TextField from)
    {
        return new TextFieldDto { Name = from.Name, Required = from.Required };
    }

    private static BoolField Map(BoolFieldDto from)
    {
        return new BoolField { Name = from.Name, Required = from.Required };
    }

    private static NumberField Map(NumberFieldDto from)
    {
        return new NumberField { Name = from.Name, Required = from.Required };
    }

    private static TextField Map(TextFieldDto from)
    {
        return new TextField { Name = from.Name, Required = from.Required };
    }
}
