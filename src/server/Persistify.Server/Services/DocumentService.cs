using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Documents;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly CreateDocumentCommand _createDocumentCommand;
    private readonly DeleteDocumentCommand _deleteDocumentCommand;
    private readonly GetDocumentCommand _getDocumentCommand;
    private readonly SearchDocumentsCommand _searchDocumentsCommand;

    public DocumentService(
        CreateDocumentCommand createDocumentCommand,
        GetDocumentCommand getDocumentCommand,
        SearchDocumentsCommand searchDocumentsCommand,
        DeleteDocumentCommand deleteDocumentCommand
    )
    {
        _createDocumentCommand = createDocumentCommand;
        _getDocumentCommand = getDocumentCommand;
        _searchDocumentsCommand = searchDocumentsCommand;
        _deleteDocumentCommand = deleteDocumentCommand;
    }

    [Authorize]
    public async ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request,
        CallContext callContext)
    {
        return await _createDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext callContext)
    {
        return await _getDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request,
        CallContext callContext)
    {
        return await _searchDocumentsCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request,
        CallContext callContext)
    {
        return await _deleteDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }
}
