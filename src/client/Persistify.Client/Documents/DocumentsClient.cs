using Persistify.Client.Core;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Documents;

public class DocumentsClient : SubClient<IDocumentService>, IDocumentsClient
{
    internal DocumentsClient(PersistifyClient persistifyClient) : base(persistifyClient)
    {
    }

    public async Task<CreateDocumentResponse> CreateDocumentAsync(IDocumentService documentService,
        CreateDocumentRequest createDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<CreateDocumentResponse>(
            async cc => await documentService.CreateDocumentAsync(createDocumentRequest, cc), callContext
        );
    }

    public async Task<GetDocumentResponse> GetDocumentAsync(IDocumentService documentService,
        GetDocumentRequest getDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<GetDocumentResponse>(
            async cc => await documentService.GetDocumentAsync(getDocumentRequest, cc), callContext
        );
    }

    public async Task<SearchDocumentsResponse> SearchDocumentsAsync(IDocumentService documentService,
        SearchDocumentsRequest searchDocumentsRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<SearchDocumentsResponse>(
            async cc => await documentService.SearchDocumentsAsync(searchDocumentsRequest, cc), callContext
        );
    }

    public async Task<DeleteDocumentResponse> DeleteDocumentAsync(IDocumentService documentService,
        DeleteDocumentRequest deleteDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<DeleteDocumentResponse>(
            async cc => await documentService.DeleteDocumentAsync(deleteDocumentRequest, cc), callContext
        );
    }
}
