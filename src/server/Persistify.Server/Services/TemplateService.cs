using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly IRequestDispatcher _requestDispatcher;

    public TemplateService(IRequestDispatcher requestDispatcher)
    {
        _requestDispatcher = requestDispatcher;
    }

    [Authorize]
    public async ValueTask<CreateTemplateResponse> CreateTemplateAsync(
        CreateTemplateRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            CreateTemplateRequest,
            CreateTemplateResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetTemplateResponse> GetTemplateAsync(
        GetTemplateRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<GetTemplateRequest, GetTemplateResponse>(
            request,
            callContext.GetClaimsPrincipal(),
            callContext.CancellationToken
        );
    }

    [Authorize]
    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(
        ListTemplatesRequest request,
        CallContext callContext
    )
    {
        return _requestDispatcher.DispatchAsync<ListTemplatesRequest, ListTemplatesResponse>(
            request,
            callContext.GetClaimsPrincipal(),
            callContext.CancellationToken
        );
    }

    [Authorize]
    public async ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(
        DeleteTemplateRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            DeleteTemplateRequest,
            DeleteTemplateResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ExistsTemplateResponse> ExistsTemplateAsync(
        ExistsTemplateRequest request,
        CallContext callContext
    )
    {
        return await _requestDispatcher.DispatchAsync<
            ExistsTemplateRequest,
            ExistsTemplateResponse
        >(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
