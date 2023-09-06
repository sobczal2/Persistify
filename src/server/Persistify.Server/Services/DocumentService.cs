using System;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly CreateDocumentCommand _createDocumentCommand;
    private readonly GetDocumentCommand _getDocumentCommand;
    private readonly DeleteDocumentCommand _deleteDocumentCommand;

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

    public async ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request, CallContext context)
    {
        return await _createDocumentCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context)
    {
        return await _getDocumentCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext context)
    {
        return await _deleteDocumentCommand.RunInTransactionAsync(request, context.CancellationToken);
    }
}
