using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class AndSearchNode : SearchNode
{
    [ProtoMember(1)]
    public List<SearchNode> Nodes { get; set; }

    public AndSearchNode()
    {
        Nodes = new List<SearchNode>(0);
    }
}
