using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class NumberRangeSearchNode : SearchNode
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    [ProtoMember(2)]
    public double Min { get; set; }

    [ProtoMember(3)]
    public double Max { get; set; }
}
