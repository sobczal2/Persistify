using ProtoBuf;

namespace Persistify.Dtos.PresetAnalyzers;

[ProtoContract]
[ProtoInclude(1, typeof(FullAnalyzerDto))]
[ProtoInclude(2, typeof(PresetNameAnalyzerDto))]
public class AnalyzerDto
{

}
