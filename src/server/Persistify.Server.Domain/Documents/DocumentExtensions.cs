namespace Persistify.Server.Domain.Documents;

public static class DocumentExtensions
{
    public static BoolFieldValue? GetBoolFieldValueByName(this Document document, string fieldName)
    {
        var fieldValue = document.GetFieldValueByName(fieldName);
        return fieldValue as BoolFieldValue;
    }

    public static NumberFieldValue? GetNumberFieldValueByName(
        this Document document,
        string fieldName
    )
    {
        var fieldValue = document.GetFieldValueByName(fieldName);
        return fieldValue as NumberFieldValue;
    }

    public static TextFieldValue? GetTextFieldValueByName(this Document document, string fieldName)
    {
        var fieldValue = document.GetFieldValueByName(fieldName);
        return fieldValue as TextFieldValue;
    }

    public static DateTimeFieldValue? GetDateTimeFieldValueByName(this Document document, string fieldName)
    {
        var fieldValue = document.GetFieldValueByName(fieldName);
        return fieldValue as DateTimeFieldValue;
    }
}
