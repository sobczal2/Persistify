using Persistify.Dtos.Documents.FieldValues;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;

namespace Persistify.Server.Mappers.Documents;

public static class FieldValueMapper
{
    public static FieldValueDto ToDto(this FieldValue fieldValue)
    {
        return fieldValue switch
        {
            BoolFieldValue boolFieldValue
                => new BoolFieldValueDto
                {
                    FieldName = boolFieldValue.FieldName,
                    Value = boolFieldValue.Value
                },
            NumberFieldValue numberFieldValue
                => new NumberFieldValueDto
                {
                    FieldName = numberFieldValue.FieldName,
                    Value = numberFieldValue.Value
                },
            TextFieldValue textFieldValue
                => new TextFieldValueDto
                {
                    FieldName = textFieldValue.FieldName,
                    Value = textFieldValue.Value
                },
            DateFieldValue dateFieldValue
                => new DateFieldValueDto
                {
                    FieldName = dateFieldValue.FieldName,
                    Value = dateFieldValue.Value
                },
            _ => throw new InternalPersistifyException(nameof(FieldValueMapper))
        };
    }

    public static FieldValue ToDomain(this FieldValueDto fieldValueDto)
    {
        return fieldValueDto switch
        {
            BoolFieldValueDto boolFieldValue
                => new BoolFieldValue
                {
                    FieldName = boolFieldValue.FieldName,
                    Value = boolFieldValue.Value
                },
            NumberFieldValueDto numberFieldValue
                => new NumberFieldValue
                {
                    FieldName = numberFieldValue.FieldName,
                    Value = numberFieldValue.Value
                },
            TextFieldValueDto textFieldValue
                => new TextFieldValue
                {
                    FieldName = textFieldValue.FieldName,
                    Value = textFieldValue.Value
                },
            _ => throw new InternalPersistifyException(nameof(FieldValueMapper))
        };
    }
}
