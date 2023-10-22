using Persistify.Dtos.PresetAnalyzers;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.PresetAnalyzers;

[ProtoContract]
public class GetPresetAnalyzerResponse : IResponse
{
    [ProtoMember(1)]
    public PresetAnalyzerDto PresetAnalyzerDto { get; set; } = default!;
}
