using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
public class OrSearchNode : SearchNode
{
    [ProtoMember(1)]
    public List<SearchNode> Nodes { get; set; }

    public OrSearchNode()
    {
        Nodes = new List<SearchNode>(0);
    }
}
