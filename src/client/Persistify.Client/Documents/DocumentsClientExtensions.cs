using Persistify.Client.Core;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using ProtoBuf.Grpc;

namespace Persistify.Client.Documents;

public static class DocumentsClientExtensions
{
    public static async Task<CreateDocumentResponse> CreateDocumentAsync(this IPersistifyClient persistifyClient,
        CreateDocumentRequest createDocumentRequest, CallContext? callContext = default)
    {
        var documentService = persistifyClient.Documents.GetService();
        return await persistifyClient.Documents.CreateDocumentAsync(documentService, createDocumentRequest,
            callContext);
    }

    public static async Task<GetDocumentResponse> GetDocumentAsync(this IPersistifyClient persistifyClient,
        GetDocumentRequest getDocumentRequest, CallContext? callContext = default)
    {
        var documentService = persistifyClient.Documents.GetService();
        return await persistifyClient.Documents.GetDocumentAsync(documentService, getDocumentRequest, callContext);
    }

    public static async Task<SearchDocumentsResponse> SearchDocumentsAsync(this IPersistifyClient persistifyClient,
        SearchDocumentsRequest searchDocumentsRequest, CallContext? callContext = default)
    {
        var documentService = persistifyClient.Documents.GetService();
        return await persistifyClient.Documents.SearchDocumentsAsync(documentService, searchDocumentsRequest,
            callContext);
    }

    public static async Task<DeleteDocumentResponse> DeleteDocumentAsync(this IPersistifyClient persistifyClient,
        DeleteDocumentRequest deleteDocumentRequest, CallContext? callContext = default)
    {
        var documentService = persistifyClient.Documents.GetService();
        return await persistifyClient.Documents.DeleteDocumentAsync(documentService, deleteDocumentRequest,
            callContext);
    }

    public static async Task<ExistsDocumentResponse> ExistsDocumentAsync(this IPersistifyClient persistifyClient,
        ExistsDocumentRequest existsDocumentRequest, CallContext? callContext = default)
    {
        var documentService = persistifyClient.Documents.GetService();
        return await persistifyClient.Documents.ExistsDocumentAsync(documentService, existsDocumentRequest,
            callContext);
    }
}
