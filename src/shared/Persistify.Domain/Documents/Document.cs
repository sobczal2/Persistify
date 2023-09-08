using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class Document
{
    private List<BoolFieldValue>? _boolFieldValues;
    private List<NumberFieldValue>? _numberFieldValues;
    private List<TextFieldValue>? _textFieldValues;

    public Document()
    {
        TextFieldValuesByFieldName = new Dictionary<string, TextFieldValue>();
        NumberFieldValuesByFieldName = new Dictionary<string, NumberFieldValue>();
        BoolFieldValuesByFieldName = new Dictionary<string, BoolFieldValue>();
    }

    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public List<TextFieldValue> TextFieldValues
    {
        get => _textFieldValues ??= new List<TextFieldValue>(0);
        set
        {
            _textFieldValues = value;
            TextFieldValuesByFieldName.Clear();
            for (var i = 0; i < _textFieldValues.Count; i++)
            {
                TextFieldValuesByFieldName.Add(_textFieldValues[i].FieldName, _textFieldValues[i]);
            }
        }
    }

    [ProtoMember(3)]
    public List<NumberFieldValue> NumberFieldValues
    {
        get => _numberFieldValues ??= new List<NumberFieldValue>(0);
        set
        {
            _numberFieldValues = value;
            NumberFieldValuesByFieldName.Clear();
            for (var i = 0; i < _numberFieldValues.Count; i++)
            {
                NumberFieldValuesByFieldName.Add(_numberFieldValues[i].FieldName, _numberFieldValues[i]);
            }
        }
    }

    [ProtoMember(4)]
    public List<BoolFieldValue> BoolFieldValues
    {
        get => _boolFieldValues ??= new List<BoolFieldValue>(0);
        set
        {
            _boolFieldValues = value;
            BoolFieldValuesByFieldName.Clear();
            for (var i = 0; i < _boolFieldValues.Count; i++)
            {
                BoolFieldValuesByFieldName.Add(_boolFieldValues[i].FieldName, _boolFieldValues[i]);
            }
        }
    }

    public IDictionary<string, TextFieldValue> TextFieldValuesByFieldName { get; }

    public IDictionary<string, NumberFieldValue> NumberFieldValuesByFieldName { get; }

    public IDictionary<string, BoolFieldValue> BoolFieldValuesByFieldName { get; }
}
