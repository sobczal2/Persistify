using System.Runtime.Serialization;
using Persistify.Protos.Common;

namespace Persistify.Protos.Templates.Requests;

[DataContract]
public class ListTemplatesRequest
{
    [DataMember(Order = 1)] public Pagination Pagination { get; set; } = default!;
}
