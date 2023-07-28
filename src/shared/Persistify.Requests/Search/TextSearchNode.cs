using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class TextSearchNode : SearchNode
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    [ProtoMember(2)]
    public string Value { get; set; } = default!;
}
