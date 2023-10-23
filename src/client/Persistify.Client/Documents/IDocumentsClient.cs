using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Documents;

public interface IDocumentsClient
{
    IDocumentService GetService();

    Task<CreateDocumentResponse> CreateDocumentAsync(IDocumentService documentService,
        CreateDocumentRequest createDocumentRequest, CallContext? callContext = default);

    Task<GetDocumentResponse> GetDocumentAsync(IDocumentService documentService, GetDocumentRequest getDocumentRequest,
        CallContext? callContext = default);

    Task<SearchDocumentsResponse> SearchDocumentsAsync(IDocumentService documentService,
        SearchDocumentsRequest searchDocumentsRequest, CallContext? callContext = default);

    Task<DeleteDocumentResponse> DeleteDocumentAsync(IDocumentService documentService,
        DeleteDocumentRequest deleteDocumentRequest, CallContext? callContext = default);

    Task<ExistsDocumentResponse> ExistsDocumentAsync(IDocumentService documentService,
        ExistsDocumentRequest existsDocumentRequest, CallContext? callContext = default);
}
