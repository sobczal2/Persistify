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

    public DocumentService(
        CreateDocumentCommand createDocumentCommand
    )
    {
        _createDocumentCommand = createDocumentCommand;
    }

    public async ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request, CallContext context)
    {
        return await _createDocumentCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
