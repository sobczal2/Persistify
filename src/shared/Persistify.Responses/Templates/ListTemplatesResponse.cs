using System.Collections.Generic;
using Persistify.Domain.Templates;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class ListTemplatesResponse
{
    public ListTemplatesResponse(IEnumerable<Template> templates, int totalCount)
    {
        Templates = templates;
        TotalCount = totalCount;
    }

    [ProtoMember(1)] public IEnumerable<Template> Templates { get; set; } = default!;

    [ProtoMember(2)] public int TotalCount { get; set; }
}
