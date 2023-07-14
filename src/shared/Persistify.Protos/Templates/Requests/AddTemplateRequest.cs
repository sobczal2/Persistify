using System.Runtime.Serialization;
using Persistify.Protos.Templates.Shared;

namespace Persistify.Protos.Templates.Requests;

[DataContract]
public class AddTemplateRequest
{
    public AddTemplateRequest()
    {
        Template = null!;
    }

    [DataMember(Order = 1)] public Template Template { get; set; }
}
