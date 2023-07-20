using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Persistify.Protos.PostingList;

[DataContract]
public class PostingList
{
    [DataMember(Order = 1)]
    public long Id { get; set; }
    [DataMember(Order = 2)]
    public string Value { get; set; }
    [DataMember(Order = 3)]
    public List<PostingListNode> Nodes { get; set; }

    public PostingList(long id, string value, List<PostingListNode> nodes)
    {
        Id = id;
        Value = value;
        Nodes = nodes;
    }

    public PostingList()
    {
        Id = 0;
        Value = string.Empty;
        Nodes = new List<PostingListNode>();
    }
}
