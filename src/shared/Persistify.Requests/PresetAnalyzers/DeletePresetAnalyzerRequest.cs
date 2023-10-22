using Persistify.Requests.Common;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf;

namespace Persistify.Requests.PresetAnalyzers;

[ProtoContract]
public class DeletePresetAnalyzerRequest : IRequest<DeletePresetAnalyzerResponse>
{
    [ProtoMember(1)]
    public string PresetAnalyzerName { get; set; } = default!;
}
