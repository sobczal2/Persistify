using System.Collections.Generic;
using System.Runtime.Serialization;
using Persistify.Protos.Templates.Shared;

namespace Persistify.Protos.Templates.Responses;

[DataContract]
public class ListTemplatesResponse
{
    public ListTemplatesResponse(List<Template> templates, long totalCount)
    {
        Templates = templates;
        TotalCount = totalCount;
    }

    [DataMember(Order = 1)] public List<Template> Templates { get; set; }

    [DataMember(Order = 2)] public long TotalCount { get; set; }
}
