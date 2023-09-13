using System;
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

    public DocumentService(
        CreateDocumentCommand createDocumentCommand,
        GetDocumentCommand getDocumentCommand,
        // ListDocumentsCommand listDocumentsCommand,
        DeleteDocumentCommand deleteDocumentCommand
    )
    {
        _createDocumentCommand = createDocumentCommand;
        _getDocumentCommand = getDocumentCommand;
        _deleteDocumentCommand = deleteDocumentCommand;
    }

    [Authorize]
    public async ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request,
        CallContext callContext)
    {
        return await _createDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext callContext)
    {
        return await _getDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext callContext)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    public async ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request,
        CallContext callContext)
    {
        return await _deleteDocumentCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
