using ProtoBuf;

namespace Persistify.Dtos.Templates.Common;

[ProtoContract]
[ProtoInclude(1, typeof(FullAnalyzerDescriptorDto))]
[ProtoInclude(2, typeof(PresetAnalyzerDescriptorDto))]
public abstract class AnalyzerDescriptorDto
{

}
