using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
[ProtoInclude(100, typeof(FullAnalyzerDescriptor))]
[ProtoInclude(101, typeof(PresetAnalyzerDescriptor))]
public abstract class AnalyzerDescriptor
{
}
