using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class NotSearchNode : SearchNode
{
    public SearchNode Node { get; set; } = default!;
}
