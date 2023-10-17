using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class ExistsDocumentResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
