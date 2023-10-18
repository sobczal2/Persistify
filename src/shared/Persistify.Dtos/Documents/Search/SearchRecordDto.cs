using System.Collections.Generic;
using Persistify.Dtos.Documents.Common;
using ProtoBuf;

namespace Persistify.Dtos.Documents.Search;

[ProtoContract]
public class SearchRecordDto
{
    [ProtoMember(1)]
    public DocumentDto Document { get; set; } = default!;

    [ProtoMember(2)]
    public List<SearchMetadataDto> MetadataList { get; set; } = default!;
}
