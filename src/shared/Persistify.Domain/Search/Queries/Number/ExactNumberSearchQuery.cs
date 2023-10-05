using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Number;

[ProtoContract]
public class ExactNumberSearchQuery : NumberSearchQuery
{
    [ProtoMember(2)]
    public double Value { get; set; }

    [ProtoMember(3)]
    public string FieldName { get; set; } = null!;

    public override string GetFieldName()
    {
        return FieldName;
    }
}
