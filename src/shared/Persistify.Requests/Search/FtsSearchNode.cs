using Persistify.Domain.SearchModes;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class FtsSearchNode : SearchNode
{
    [ProtoMember(1)]
    public string FieldName { get; set; } = default!;

    [ProtoMember(2)]
    public string Value { get; set; } = default!;

    [ProtoMember(3)]
    public FtsSearchMode Mode { get; set; }
}
