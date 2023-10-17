using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.CommandHandlers.Documents;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly IRequestDispatcher _requestDispatcher;

    public DocumentService(
        IRequestDispatcher requestDispatcher
    )
    {
        _requestDispatcher = requestDispatcher;
    }

    [Authorize]
    public async ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<CreateDocumentRequest, CreateDocumentResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<GetDocumentRequest, GetDocumentResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<SearchDocumentsRequest, SearchDocumentsResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<DeleteDocumentRequest, DeleteDocumentResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ExistsDocumentResponse> ExistsDocumentAsync(ExistsDocumentRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<ExistsDocumentRequest, ExistsDocumentResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
