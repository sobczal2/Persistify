using Persistify.Requests.Shared;
using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class ListTemplatesRequest
{
    [ProtoMember(1)]
    public Pagination Pagination { get; set; } = default!;
}
