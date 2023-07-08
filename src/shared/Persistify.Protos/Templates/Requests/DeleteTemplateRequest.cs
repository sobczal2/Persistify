using System.Runtime.Serialization;

namespace Persistify.Protos.Templates.Requests;

[DataContract]
public class DeleteTemplateRequest
{
    [DataMember(Order = 1)]
    public string TemplateName { get; set; } = default!;
}
