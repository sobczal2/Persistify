using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;

namespace Persistify.Server.Persistence.DataStructures.Trees;

[ProtoContract]
public class BTreeLeafNode<TKey, TItem> : IBTreeNode
    where TKey : notnull
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public int ParentId { get; set; }

    [ProtoMember(3)]
    public int LeftSiblingId { get; set; }

    [ProtoMember(4)]
    public int RightSiblingId { get; set; }

    [ProtoMember(5)]
    public SortedList<TKey, List<TItem>> Items { get; set; }

    public BTreeLeafNode()
    {
        Items = new SortedList<TKey, List<TItem>>();
    }
}
