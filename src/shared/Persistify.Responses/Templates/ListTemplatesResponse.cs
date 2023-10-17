using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class ListTemplatesResponse : IResponse
{
    [ProtoMember(1)]
    public List<Template> Templates { get; set; } = default!;

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
