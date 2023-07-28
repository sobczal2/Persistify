using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProtoBuf;

namespace Persistify.Server.Persistence.DataStructures.Trees;

[ProtoContract]
public class BTreeInternalNode<TKey> : IBTreeNode
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
    public List<TKey> Keys { get; set; }

    [ProtoMember(6)]
    public List<(long id, bool leaf)> ChildrenIds { get; set; }


    public BTreeInternalNode()
    {
        Keys = new List<TKey>();
        ChildrenIds = new List<(long id, bool leaf)>();
    }

    public (long id, bool leaf) GetChildId(TKey key, IComparer<TKey> comparer)
    {
        if (Keys.Count == 0)
        {
            return ChildrenIds[0];
        }

        var start = 0;
        var end = Keys.Count - 1;
        while (start <= end)
        {
            var mid = (start + end) / 2;
            var comparison = comparer.Compare(key, Keys[mid]);
            if (comparison == 0)
            {
                return ChildrenIds[mid + 1];
            }

            if (comparison < 0)
            {
                end = mid - 1;
            }
            else
            {
                start = mid + 1;
            }
        }

        return ChildrenIds[start];
    }

    public override bool Equals(object? obj)
    {
        if (obj is BTreeInternalNode<TKey> other)
        {
            return Equals(other);
        }

        return false;
    }

    protected bool Equals(BTreeInternalNode<TKey> other)
    {
        return Id == other.Id && ParentId == other.ParentId && LeftSiblingId == other.LeftSiblingId &&
               RightSiblingId == other.RightSiblingId && Keys.Equals(other.Keys) &&
               ChildrenIds.Equals(other.ChildrenIds);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ParentId, LeftSiblingId, RightSiblingId, Keys, ChildrenIds);
    }
}
