using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.PresetAnalyzers;

public interface IPresetAnalyzersClient
{
    IPresetAnalyzerService GetService();

    Task<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        CreatePresetAnalyzerRequest createPresetAnalyzerRequest, CallContext? callContext = default);

    Task<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        GetPresetAnalyzerRequest getPresetAnalyzerRequest, CallContext? callContext = default);

    Task<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(IPresetAnalyzerService presetAnalyzerService,
        ListPresetAnalyzersRequest listPresetAnalyzersRequest, CallContext? callContext = default);

    Task<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        DeletePresetAnalyzerRequest deletePresetAnalyzerRequest, CallContext? callContext = default);

    Task<ExistsPresetAnalyzerResponse> ExistsPresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        ExistsPresetAnalyzerRequest existsPresetAnalyzerRequest, CallContext? callContext = default);
}
