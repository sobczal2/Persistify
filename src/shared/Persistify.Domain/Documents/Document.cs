using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Domain.Documents;

[ProtoContract]
public class Document
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public List<FieldValue> FieldValues { get; set; } = default!;

    private Dictionary<string, FieldValue>? _fieldNameTypeMap;

    private void EnsureFieldNameTypeMapInitialized()
    {
        if (_fieldNameTypeMap == null)
        {
            _fieldNameTypeMap = FieldValues.ToDictionary(f => f.FieldName, f => f);
        }
    }

    public FieldValue? GetFieldValueByName(string fieldName)
    {
        EnsureFieldNameTypeMapInitialized();
        return _fieldNameTypeMap!.TryGetValue(fieldName, out var fieldValue) ? fieldValue : null;
    }
}
