using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Bool;

[ProtoContract]
public class ExactBoolSearchQuery : BoolSearchQuery
{
    [ProtoMember(2)]
    public bool Value { get; set; }

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }
}
