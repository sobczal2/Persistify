using System.Runtime.Serialization;
using Persistify.Protos.Templates.Shared;

namespace Persistify.Protos.Templates.Requests;

[DataContract]
public class AddTemplateRequest
{
    [DataMember(Order = 1)]
    public Template Template { get; set; } = default!;
}
