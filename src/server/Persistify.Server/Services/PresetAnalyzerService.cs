using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    public async ValueTask<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(
        CreatePresetAnalyzerRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            CreatePresetAnalyzerRequest,
            CreatePresetAnalyzerResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(
        GetPresetAnalyzerRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            GetPresetAnalyzerRequest,
            GetPresetAnalyzerResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(
        ListPresetAnalyzersRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            ListPresetAnalyzersRequest,
            ListPresetAnalyzersResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(
        DeletePresetAnalyzerRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            DeletePresetAnalyzerRequest,
            DeletePresetAnalyzerResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ExistsPresetAnalyzerResponse> ExistsPresetAnalyzerAsync(
        ExistsPresetAnalyzerRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            ExistsPresetAnalyzerRequest,
            ExistsPresetAnalyzerResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
