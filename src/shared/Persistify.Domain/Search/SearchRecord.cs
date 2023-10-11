using System.Collections.Generic;
using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Domain.Search;

[ProtoContract]
public class SearchRecord
{
    [ProtoMember(1)]
    public Document Document { get; set; } = default!;

    [ProtoMember(2)]
    public List<SearchMetadata> MetadataList { get; set; } = default!;

    public SearchRecord()
    {

    }

    public SearchRecord(Document document, List<SearchMetadata> metadataList)
    {
        Document = document;
        MetadataList = metadataList;
    }

    public SearchRecord(Document document, SearchMetadata metadata)
    {
        Document = document;
        MetadataList = new List<SearchMetadata> {metadata};
    }
}
