using System.Collections.Generic;

namespace Persistify.Protos.PostingList;

public class PostingListNode
{
    public string Value { get; set; }
    public List<DocumentAssociation> Documents { get; set; }

    public PostingListNode(string value, List<DocumentAssociation> documents)
    {
        Value = value;
        Documents = documents;
    }
}
