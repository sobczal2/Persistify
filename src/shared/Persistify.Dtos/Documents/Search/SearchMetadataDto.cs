using ProtoBuf;

namespace Persistify.Dtos.Documents.Search;

[ProtoContract]
public class SearchMetadataDto
{
    [ProtoMember(1)]
    public string Name { get; set; } = default!;

    [ProtoMember(2)]
    public string Value { get; set; } = default!;
}
