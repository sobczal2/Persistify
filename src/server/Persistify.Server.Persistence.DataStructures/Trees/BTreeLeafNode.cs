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
    public long Id { get; set; }

    [ProtoMember(2)]
    public long ParentId { get; set; }

    [ProtoMember(3)]
    public long LeftSiblingId { get; set; }

    [ProtoMember(4)]
    public long RightSiblingId { get; set; }

    [ProtoMember(5)]
    public SortedList<TKey, List<TItem>> Items { get; set; }

    public BTreeLeafNode()
    {
        Items = new SortedList<TKey, List<TItem>>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is BTreeLeafNode<TKey, TItem> other)
        {
            return Equals(other);
        }

        return false;
    }

    protected bool Equals(BTreeLeafNode<TKey, TItem> other)
    {
        return Id == other.Id && ParentId == other.ParentId && LeftSiblingId == other.LeftSiblingId &&
               RightSiblingId == other.RightSiblingId && Items.Equals(other.Items);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ParentId, LeftSiblingId, RightSiblingId, Items);
    }
}
