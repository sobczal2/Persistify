using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Text;

[ProtoContract]
public class FullTextSearchQuery : TextSearchQuery
{
    public FullTextSearchQuery()
    {
        Value = null!;
    }

    [ProtoMember(11)]
    public string Value { get; set; }
}
