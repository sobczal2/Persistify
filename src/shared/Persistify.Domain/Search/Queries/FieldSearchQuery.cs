using ProtoBuf;

namespace Persistify.Domain.Search.Queries;

[ProtoContract]
public class FieldSearchQuery : SearchQuery
{
    public FieldSearchQuery()
    {
        FieldName = null!;
    }

    [ProtoMember(2)]
    public string FieldName { get; set; }
}
