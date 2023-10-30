using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.PresetAnalyzers;

public static class PresetAnalyzersClientExtensions
{
    public static async Task<Result<CreatePresetAnalyzerResponse>> CreatePresetAnalyzerAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        CreatePresetAnalyzerRequest createPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        var presetAnalyzerService = persistifyLowLevelClient.PresetAnalyzers.GetService();
        return await persistifyLowLevelClient.PresetAnalyzers.CreatePresetAnalyzerAsync(
            presetAnalyzerService,
            createPresetAnalyzerRequest,
            callContext
        );
    }

    public static async Task<Result<GetPresetAnalyzerResponse>> GetPresetAnalyzerAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        GetPresetAnalyzerRequest getPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        var presetAnalyzerService = persistifyLowLevelClient.PresetAnalyzers.GetService();
        return await persistifyLowLevelClient.PresetAnalyzers.GetPresetAnalyzerAsync(
            presetAnalyzerService,
            getPresetAnalyzerRequest,
            callContext
        );
    }

    public static async Task<Result<ListPresetAnalyzersResponse>> ListPresetAnalyzersAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ListPresetAnalyzersRequest listPresetAnalyzersRequest,
        CallContext? callContext = default
    )
    {
        var presetAnalyzerService = persistifyLowLevelClient.PresetAnalyzers.GetService();
        return await persistifyLowLevelClient.PresetAnalyzers.ListPresetAnalyzersAsync(
            presetAnalyzerService,
            listPresetAnalyzersRequest,
            callContext
        );
    }

    public static async Task<Result<DeletePresetAnalyzerResponse>> DeletePresetAnalyzerAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        DeletePresetAnalyzerRequest deletePresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        var presetAnalyzerService = persistifyLowLevelClient.PresetAnalyzers.GetService();
        return await persistifyLowLevelClient.PresetAnalyzers.DeletePresetAnalyzerAsync(
            presetAnalyzerService,
            deletePresetAnalyzerRequest,
            callContext
        );
    }

    public static async Task<Result<ExistsPresetAnalyzerResponse>> ExistsPresetAnalyzerAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ExistsPresetAnalyzerRequest existsPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        var presetAnalyzerService = persistifyLowLevelClient.PresetAnalyzers.GetService();
        return await persistifyLowLevelClient.PresetAnalyzers.ExistsPresetAnalyzerAsync(
            presetAnalyzerService,
            existsPresetAnalyzerRequest,
            callContext
        );
    }
}
