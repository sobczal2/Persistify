using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class Template
{
    private List<BoolField>? _booleanFields;
    private IDictionary<string, BoolField>? _booleanFieldsByName;
    private List<NumberField>? _numberFields;
    private IDictionary<string, NumberField>? _numberFieldsByName;
    private List<TextField>? _textFields;

    private IDictionary<string, TextField>? _textFieldsByName;

    [ProtoMember(1)] public int Id { get; set; }

    [ProtoMember(2)] public string Name { get; set; } = default!;

    [ProtoMember(3)]
    public List<TextField> TextFields
    {
        get => _textFields ??= new List<TextField>(0);
        set
        {
            _textFields = value;
            _textFieldsByName = null;
        }
    }

    [ProtoMember(4)]
    public List<NumberField> NumberFields
    {
        get => _numberFields ??= new List<NumberField>(0);
        set
        {
            _numberFields = value;
            _numberFieldsByName = null;
        }
    }

    [ProtoMember(5)]
    public List<BoolField> BooleanFields
    {
        get => _booleanFields ??= new List<BoolField>(0);
        set
        {
            _booleanFields = value;
            _booleanFieldsByName = null;
        }
    }

    public IDictionary<string, TextField> TextFieldsByName =>
        _textFieldsByName ??= TextFields.ToDictionary(x => x.Name);

    public IDictionary<string, NumberField> NumberFieldsByName =>
        _numberFieldsByName ??= NumberFields.ToDictionary(x => x.Name);

    public IDictionary<string, BoolField> BoolFieldsByName =>
        _booleanFieldsByName ??= BooleanFields.ToDictionary(x => x.Name);
}
