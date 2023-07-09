using System.Runtime.Serialization;

namespace Persistify.Protos.Common;

[DataContract]
public class Pagination
{
    [DataMember(Order = 1)]
    public int PageNumber { get; set; }
    [DataMember(Order = 2)]
    public int PageSize { get; set; }
}
