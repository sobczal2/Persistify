using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class OrSearchNode : SearchNode
{
    public OrSearchNode()
    {
        Nodes = new List<SearchNode>(0);
    }

    [ProtoMember(1)]
    public List<SearchNode> Nodes { get; set; }
}
