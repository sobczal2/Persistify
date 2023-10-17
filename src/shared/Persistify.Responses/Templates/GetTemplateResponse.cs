using Persistify.Domain.Templates;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class GetTemplateResponse : IResponse
{
    [ProtoMember(1)]
    public Template Template { get; set; } = default!;
}
