using System.Runtime.Serialization;

namespace Persistify.Protos.Common;

[DataContract]
public class Pagination
{
    [DataMember(Order = 1)]
    uint PageNumber { get; set; }
    [DataMember(Order = 2)]
    uint PageSize { get; set; }
}
