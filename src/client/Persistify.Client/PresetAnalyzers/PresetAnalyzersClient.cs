using Persistify.Client.Core;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.PresetAnalyzers;

public class PresetAnalyzersClient : SubClient<IPresetAnalyzerService>, IPresetAnalyzersClient
{
    internal PresetAnalyzersClient(PersistifyClient persistifyClient) : base(persistifyClient)
    {
    }

    public async Task<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        CreatePresetAnalyzerRequest createPresetAnalyzerRequest, CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<CreatePresetAnalyzerResponse>(
            async cc => await presetAnalyzerService.CreatePresetAnalyzerAsync(createPresetAnalyzerRequest, cc), callContext
        );
    }

    public async Task<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        GetPresetAnalyzerRequest getPresetAnalyzerRequest, CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<GetPresetAnalyzerResponse>(
            async cc => await presetAnalyzerService.GetPresetAnalyzerAsync(getPresetAnalyzerRequest, cc), callContext
        );
    }

    public async Task<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(IPresetAnalyzerService presetAnalyzerService,
        ListPresetAnalyzersRequest listPresetAnalyzersRequest, CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<ListPresetAnalyzersResponse>(
            async cc => await presetAnalyzerService.ListPresetAnalyzersAsync(listPresetAnalyzersRequest, cc), callContext
        );
    }

    public async Task<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        DeletePresetAnalyzerRequest deletePresetAnalyzerRequest, CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<DeletePresetAnalyzerResponse>(
            async cc => await presetAnalyzerService.DeletePresetAnalyzerAsync(deletePresetAnalyzerRequest, cc), callContext
        );
    }

    public async Task<ExistsPresetAnalyzerResponse> ExistsPresetAnalyzerAsync(IPresetAnalyzerService presetAnalyzerService,
        ExistsPresetAnalyzerRequest existsPresetAnalyzerRequest, CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<ExistsPresetAnalyzerResponse>(
            async cc => await presetAnalyzerService.ExistsPresetAnalyzerAsync(existsPresetAnalyzerRequest, cc), callContext
        );
    }
}
