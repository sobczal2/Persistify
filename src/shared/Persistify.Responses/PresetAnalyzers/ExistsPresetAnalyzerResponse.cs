using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.PresetAnalyzers;

[ProtoContract]
public class ExistsPresetAnalyzerResponse : IResponse
{
    [ProtoMember(1)]
    public bool Exists { get; set; }
}
