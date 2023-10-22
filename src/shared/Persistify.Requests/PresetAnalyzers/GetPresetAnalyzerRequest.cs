using Persistify.Requests.Common;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Requests.PresetAnalyzers;

public class GetPresetAnalyzerRequest : IRequest<GetPresetAnalyzerResponse>
{
    [ProtoMember(1)]
    public string PresetAnalyzerName { get; set; } = default!;
}
