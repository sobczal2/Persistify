using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Documents;

public interface IDocumentsClient
{
    IDocumentService GetService();

    Task<Result<CreateDocumentResponse>> CreateDocumentAsync(
        IDocumentService documentService,
        CreateDocumentRequest createDocumentRequest,
        CallContext? callContext = default
    );

    Task<Result<GetDocumentResponse>> GetDocumentAsync(
        IDocumentService documentService,
        GetDocumentRequest getDocumentRequest,
        CallContext? callContext = default
    );

    Task<Result<SearchDocumentsResponse>> SearchDocumentsAsync(
        IDocumentService documentService,
        SearchDocumentsRequest searchDocumentsRequest,
        CallContext? callContext = default
    );

    Task<Result<DeleteDocumentResponse>> DeleteDocumentAsync(
        IDocumentService documentService,
        DeleteDocumentRequest deleteDocumentRequest,
        CallContext? callContext = default
    );

    Task<Result<ExistsDocumentResponse>> ExistsDocumentAsync(
        IDocumentService documentService,
        ExistsDocumentRequest existsDocumentRequest,
        CallContext? callContext = default
    );
}
