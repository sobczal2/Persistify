using System.Threading.Tasks;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class PresetAnalyzerService : IPresetAnalyzerService
{
    private readonly IRequestDispatcher _requestDispatcher;

    public PresetAnalyzerService(
        IRequestDispatcher requestDispatcher
    )
    {
        _requestDispatcher = requestDispatcher;
    }

    public async ValueTask<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(CreatePresetAnalyzerRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<CreatePresetAnalyzerRequest, CreatePresetAnalyzerResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    public async ValueTask<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(GetPresetAnalyzerRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<GetPresetAnalyzerRequest, GetPresetAnalyzerResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    public async ValueTask<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(ListPresetAnalyzersRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<ListPresetAnalyzersRequest, ListPresetAnalyzersResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    public async ValueTask<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(DeletePresetAnalyzerRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<DeletePresetAnalyzerRequest, DeletePresetAnalyzerResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
