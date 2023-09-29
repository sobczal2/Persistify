using ProtoBuf;

namespace Persistify.Domain.Search.Queries.Bool;

[ProtoContract]
public class BoolSearchQuery : FieldSearchQuery
{
    [ProtoMember(11)]
    public bool Value { get; set; }
}
