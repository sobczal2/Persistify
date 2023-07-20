using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Domain.Templates;

[ProtoContract]
public class AnalyzerDescriptor
{
    [ProtoMember(1)] public string AnalyzerName { get; set; } = default!;
    [ProtoMember(2)] public List<string> CharacterFilterNames { get; set; } = default!;
    [ProtoMember(3)] public string TokenizerName { get; set; } = default!;
    [ProtoMember(4)] public List<string> TokenFilterNames { get; set; } = default!;
}
