using Persistify.Requests.Shared;
using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class SearchDocumentsRequest
{
    [ProtoMember(1)]
    public int TemplateId { get; set; }

    [ProtoMember(2)]
    public Pagination Pagination { get; set; } = default!;

    // TODO: Add search parameters
}
