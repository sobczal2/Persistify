using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class DeleteDocumentRequest
{
    [ProtoMember(1)] public string TemplateName { get; set; } = default!;

    [ProtoMember(2)] public int DocumentId { get; set; }
}
