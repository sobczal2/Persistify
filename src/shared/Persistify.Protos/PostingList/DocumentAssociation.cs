using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Persistify.Protos.PostingList;

[DataContract]
public class DocumentAssociation
{
    [DataMember(Order = 1)]
    public long DocumentId { get; set; }
    [DataMember(Order = 2)]
    public string FieldName { get; set; }
    [DataMember(Order = 3)]
    public List<int> Positions { get; set; }

    public DocumentAssociation(long documentId, string fieldName, List<int> positions)
    {
        DocumentId = documentId;
        FieldName = fieldName;
        Positions = positions;
    }
}
