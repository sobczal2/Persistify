using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.PresetAnalyzers;

public class PresetAnalyzersClient : SubClient<IPresetAnalyzerService>, IPresetAnalyzersClient
{
    internal PresetAnalyzersClient(PersistifyLowLevelClient persistifyLowLevelClient)
        : base(persistifyLowLevelClient) { }

    public async Task<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(
        IPresetAnalyzerService presetAnalyzerService,
        CreatePresetAnalyzerRequest createPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<CreatePresetAnalyzerResponse>(
            async cc =>
                await Result<CreatePresetAnalyzerResponse>.FromAsync(
                    async () =>
                        await presetAnalyzerService.CreatePresetAnalyzerAsync(
                            createPresetAnalyzerRequest,
                            cc
                        )
                ),
            callContext
        );
    }

    public async Task<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(
        IPresetAnalyzerService presetAnalyzerService,
        GetPresetAnalyzerRequest getPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<GetPresetAnalyzerResponse>(
            async cc =>
                await Result<GetPresetAnalyzerResponse>.FromAsync(
                    async () =>
                        await presetAnalyzerService.GetPresetAnalyzerAsync(
                            getPresetAnalyzerRequest,
                            cc
                        )
                ),
            callContext
        );
    }

    public async Task<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(
        IPresetAnalyzerService presetAnalyzerService,
        ListPresetAnalyzersRequest listPresetAnalyzersRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ListPresetAnalyzersResponse>(
            async cc =>
                await Result<ListPresetAnalyzersResponse>.FromAsync(
                    async () =>
                        await presetAnalyzerService.ListPresetAnalyzersAsync(
                            listPresetAnalyzersRequest,
                            cc
                        )
                ),
            callContext
        );
    }

    public async Task<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(
        IPresetAnalyzerService presetAnalyzerService,
        DeletePresetAnalyzerRequest deletePresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<DeletePresetAnalyzerResponse>(
            async cc =>
                await Result<DeletePresetAnalyzerResponse>.FromAsync(
                    async () =>
                        await presetAnalyzerService.DeletePresetAnalyzerAsync(
                            deletePresetAnalyzerRequest,
                            cc
                        )
                ),
            callContext
        );
    }

    public async Task<ExistsPresetAnalyzerResponse> ExistsPresetAnalyzerAsync(
        IPresetAnalyzerService presetAnalyzerService,
        ExistsPresetAnalyzerRequest existsPresetAnalyzerRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ExistsPresetAnalyzerResponse>(
            async cc =>
                await Result<ExistsPresetAnalyzerResponse>.FromAsync(
                    async () =>
                        await presetAnalyzerService.ExistsPresetAnalyzerAsync(
                            existsPresetAnalyzerRequest,
                            cc
                        )
                ),
            callContext
        );
    }
}
