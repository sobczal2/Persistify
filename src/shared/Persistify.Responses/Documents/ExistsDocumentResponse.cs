using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Documents;

[ProtoContract]
public class ExistsDocumentResponse : IResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
