using ProtoBuf;

namespace Persistify.Domain.Search;

[ProtoContract]
public class SearchMetadata
{
    [ProtoMember(1)]
    public string Name { get; set; } = null!;

    [ProtoMember(2)]
    public string Value { get; set; } = null!;
}
