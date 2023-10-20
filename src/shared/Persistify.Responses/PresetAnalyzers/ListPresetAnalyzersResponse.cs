using System.Collections.Generic;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Common;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.PresetAnalyzers;

[ProtoContract]
public class ListPresetAnalyzersResponse : IResponse
{
    [ProtoMember(1)]
    public List<PresetAnalyzerDto> PresetAnalyzerDtos { get; set; }

    [ProtoMember(2)]
    public int TotalCount { get; set; }

    public ListPresetAnalyzersResponse()
    {
        PresetAnalyzerDtos = new List<PresetAnalyzerDto>();
    }
}
