using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class Document
{
    public Document()
    {
        TextFields = Array.Empty<TextField>();
        NumberFields = Array.Empty<NumberField>();
        BoolFields = Array.Empty<BoolField>();
    }

    [DataMember(Order = 1)] public TextField[] TextFields { get; set; }

    [DataMember(Order = 2)] public NumberField[] NumberFields { get; set; }

    [DataMember(Order = 3)] public BoolField[] BoolFields { get; set; }

    public IDictionary<string, TextField> GetTextFieldsDict()
    {
        var dict = new Dictionary<string, TextField>(TextFields.Length);

        for (var i = 0; i < TextFields.Length; i++)
        {
            var textField = TextFields[i];
            dict.Add(textField.FieldName, textField);
        }

        return dict;
    }

    public IDictionary<string, NumberField> GetNumberFieldsDict()
    {
        var dict = new Dictionary<string, NumberField>(NumberFields.Length);

        for (var i = 0; i < NumberFields.Length; i++)
        {
            var numberField = NumberFields[i];
            dict.Add(numberField.FieldName, numberField);
        }

        return dict;
    }

    public IDictionary<string, BoolField> GetBoolFieldsDict()
    {
        var dict = new Dictionary<string, BoolField>(BoolFields.Length);

        for (var i = 0; i < BoolFields.Length; i++)
        {
            var boolField = BoolFields[i];
            dict.Add(boolField.FieldName, boolField);
        }

        return dict;
    }
}
