using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class ExistsTemplateResponse : IResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
