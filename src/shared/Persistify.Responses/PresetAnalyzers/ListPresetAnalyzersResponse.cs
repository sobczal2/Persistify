using System.Collections.Generic;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.PresetAnalyzers;

[ProtoContract]
public class ListPresetAnalyzersResponse : IResponse
{
    public ListPresetAnalyzersResponse()
    {
        PresetAnalyzerDtos = new List<PresetAnalyzerDto>();
    }

    [ProtoMember(1)]
    public List<PresetAnalyzerDto> PresetAnalyzerDtos { get; set; }

    [ProtoMember(2)]
    public int TotalCount { get; set; }
}
