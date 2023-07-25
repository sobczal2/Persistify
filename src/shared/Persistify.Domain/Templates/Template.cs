using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class Template
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = default!;

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
    private List<TextField>? _textFields;

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
    private List<NumberField>? _numberFields;

    [ProtoMember(5)]
    public List<BoolField> BoolFields
    {
        get => _boolFields ??= new List<BoolField>(0);
        set
        {
            _boolFields = value;
            _boolFieldsByName = null;
        }
    }

    private List<BoolField>? _boolFields;

    private IDictionary<string, TextField>? _textFieldsByName;
    private IDictionary<string, NumberField>? _numberFieldsByName;
    private IDictionary<string, BoolField>? _boolFieldsByName;

    public IDictionary<string, TextField> TextFieldsByName =>
        _textFieldsByName ??= TextFields.ToDictionary(x => x.Name);

    public IDictionary<string, NumberField> NumberFieldsByName =>
        _numberFieldsByName ??= NumberFields.ToDictionary(x => x.Name);

    public IDictionary<string, BoolField> BoolFieldsByName =>
        _boolFieldsByName ??= BoolFields.ToDictionary(x => x.Name);
}
