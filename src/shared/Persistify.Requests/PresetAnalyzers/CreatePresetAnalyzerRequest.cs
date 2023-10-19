using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using Persistify.Requests.Common;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Requests.PresetAnalyzers;

public class CreatePresetAnalyzerRequest : IRequest<CreatePresetAnalyzerResponse>
{
    [ProtoMember(1)]
    public string PresetAnalyzerName { get; set; } = default!;

    [ProtoMember(2)]
    public FullAnalyzerDto FullAnalyzerDto { get; set; } = default!;
}
