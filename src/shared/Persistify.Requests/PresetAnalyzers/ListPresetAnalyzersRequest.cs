using Persistify.Dtos.Common;
using Persistify.Requests.Common;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Requests.PresetAnalyzers;

[ProtoContract]
public class ListPresetAnalyzersRequest : IRequest<ListPresetAnalyzersResponse>
{
    [ProtoMember(1)]
    public PaginationDto PaginationDto { get; set; } = default!;
}
