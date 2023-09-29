using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Text;

[ProtoContract]
public class PrefixTextSearchQuery : TextSearchQuery
{
    [ProtoMember(11)]
    public float Value { get; set; }
}
