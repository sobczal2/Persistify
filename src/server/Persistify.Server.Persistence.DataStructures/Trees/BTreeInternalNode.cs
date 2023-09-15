// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using ProtoBuf;
//
// namespace Persistify.Server.Persistence.DataStructures.Trees;
//
// [ProtoContract]
// public class BTreeInternalNode<TKey> : IBTreeNode
// {
//     [ProtoMember(1)]
//     public int Id { get; set; }
//
//     [ProtoMember(2)]
//     public int ParentId { get; set; }
//
//     [ProtoMember(3)]
//     public int LeftSiblingId { get; set; }
//
//     [ProtoMember(4)]
//     public int RightSiblingId { get; set; }
//
//     [ProtoMember(5)]
//     public List<TKey> Keys { get; set; }
//
//     [ProtoMember(6)]
//     public List<(int id, bool leaf)> ChildrenIds { get; set; }
//
//
//     public BTreeInternalNode()
//     {
//         Keys = new List<TKey>();
//         ChildrenIds = new List<(int id, bool leaf)>();
//     }
//
//     public (int id, bool leaf) GetChildId(TKey key, IComparer<TKey> comparer)
//     {
//         if (Keys.Count == 0)
//         {
//             return ChildrenIds[0];
//         }
//
//         var start = 0;
//         var end = Keys.Count - 1;
//         while (start <= end)
//         {
//             var mid = (start + end) / 2;
//             var comparison = comparer.Compare(key, Keys[mid]);
//             if (comparison == 0)
//             {
//                 return ChildrenIds[mid + 1];
//             }
//
//             if (comparison < 0)
//             {
//                 end = mid - 1;
//             }
//             else
//             {
//                 start = mid + 1;
//             }
//         }
//
//         return ChildrenIds[start];
//     }
// }



