using ProtoBuf;

namespace Persistify.Dtos.Common;

[ProtoContract]
public class PaginationDto
{
    [ProtoMember(1)]
    public int PageNumber { get; set; }

    [ProtoMember(2)]
    public int PageSize { get; set; }
}
