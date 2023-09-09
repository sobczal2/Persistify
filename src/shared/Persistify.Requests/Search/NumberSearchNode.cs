using Persistify.Domain.SearchModes;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class NumberSearchNode : SearchNode
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    [ProtoMember(2)]
    public double Value { get; set; }

    [ProtoMember(3)]
    public NumberSearchMode Mode { get; set; }
}
