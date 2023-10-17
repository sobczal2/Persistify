using ProtoBuf;

namespace Persistify.Requests.Common;

[ProtoContract]
public class Pagination
{
    [ProtoMember(1)]
    public int PageNumber { get; set; }

    [ProtoMember(2)]
    public int PageSize { get; set; }
}
