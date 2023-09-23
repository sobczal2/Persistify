using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class BoolSearchNode : SearchNode
{
    [ProtoMember(1)] public string FieldName { get; set; } = default!;

    [ProtoMember(2)] public bool Value { get; set; }
    [ProtoMember(3)] public float Boost { get; set; }
}
