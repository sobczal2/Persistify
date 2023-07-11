using System.Collections.Generic;
using System.Runtime.Serialization;
using Persistify.Protos.Templates.Shared;

namespace Persistify.Protos.Templates.Responses;

[DataContract]
public class ListTemplatesResponse
{
    [DataMember(Order = 1)] public List<Template> Templates { get; set; } = default!;

    [DataMember(Order = 2)] public long TotalCount { get; set; }
}
