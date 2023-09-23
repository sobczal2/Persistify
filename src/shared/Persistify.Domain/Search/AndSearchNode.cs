using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class AndSearchNode : SearchNode
{
    public AndSearchNode()
    {
        Nodes = new List<SearchNode>(0);
    }

    [ProtoMember(1)] public List<SearchNode> Nodes { get; set; }
}
