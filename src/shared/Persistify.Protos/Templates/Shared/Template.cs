using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Shared;

[DataContract]
public class Template
{
    public Template()
    {
        Name = null!;
        Fields = Array.Empty<Field>();
    }

    [DataMember(Order = 1)] public string Name { get; set; }

    [DataMember(Order = 2)] public Field[] Fields { get; set; }

    public IDictionary<FieldType, List<Field>> GetFieldsDict()
    {
        var dict = new Dictionary<FieldType, List<Field>>(3);
        var textFields = new List<Field>();
        var numberFields = new List<Field>();
        var boolFields = new List<Field>();

        for (var i = 0; i < Fields.Length; i++)
        {
            var field = Fields[i];
            switch (field.Type)
            {
                case FieldType.Text:
                    textFields.Add(field);
                    break;
                case FieldType.Number:
                    numberFields.Add(field);
                    break;
                case FieldType.Bool:
                    boolFields.Add(field);
                    break;
            }
        }

        dict.Add(FieldType.Text, textFields);
        dict.Add(FieldType.Number, numberFields);
        dict.Add(FieldType.Bool, boolFields);

        return dict;
    }
}
