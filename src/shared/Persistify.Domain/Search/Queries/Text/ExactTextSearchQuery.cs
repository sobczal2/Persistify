using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Text;

[ProtoContract]
public class ExactTextSearchQuery : TextSearchQuery
{
    public ExactTextSearchQuery()
    {
        Value = null!;
    }

    [ProtoMember(11)]
    public string Value { get; set; }
}
