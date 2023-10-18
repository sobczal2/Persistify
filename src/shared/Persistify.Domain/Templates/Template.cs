using System.Collections.Generic;
using System.Linq;
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
    public List<Field> Fields { get; set; } = default!;

    private Dictionary<string, Field>? _fieldNameTypeMap;

    private void EnsureFieldNameTypeMapInitialized()
    {
        if (_fieldNameTypeMap == null)
        {
            _fieldNameTypeMap = Fields.ToDictionary(f => f.Name, f => f);
        }
    }

    public Field? GetFieldByName(string fieldName)
    {
        EnsureFieldNameTypeMapInitialized();
        return _fieldNameTypeMap!.TryGetValue(fieldName, out var field) ? field : null;
    }
}
