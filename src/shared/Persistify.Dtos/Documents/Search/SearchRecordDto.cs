using System.Collections.Generic;
using Persistify.Dtos.Documents.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search;

[ProtoContract]
public class SearchRecordDto
{
    [ProtoMember(1)]
    public DocumentDto DocumentDto { get; set; } = default!;

    [ProtoMember(2)]
    public List<SearchMetadataDto> MetadataList { get; set; }

    public SearchRecordDto()
    {
        MetadataList = new List<SearchMetadataDto>();
    }
}
