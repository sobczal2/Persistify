using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Documents;

public class DocumentsClient : SubClient<IDocumentService>, IDocumentsClient
{
    internal DocumentsClient(PersistifyLowLevelClient persistifyLowLevelClient) : base(persistifyLowLevelClient)
    {
    }

    public async Task<Result<CreateDocumentResponse>> CreateDocumentAsync(IDocumentService documentService,
        CreateDocumentRequest createDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<CreateDocumentResponse>(
            async cc => await Result<CreateDocumentResponse>.FromAsync(async () =>
                await documentService.CreateDocumentAsync(createDocumentRequest, cc)), callContext
        );
    }

    public async Task<Result<GetDocumentResponse>> GetDocumentAsync(IDocumentService documentService,
        GetDocumentRequest getDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<GetDocumentResponse>(
            async cc => await Result<GetDocumentResponse>.FromAsync(async () =>
                await documentService.GetDocumentAsync(getDocumentRequest, cc)), callContext
        );
    }

    public async Task<Result<SearchDocumentsResponse>> SearchDocumentsAsync(IDocumentService documentService,
        SearchDocumentsRequest searchDocumentsRequest,
        CallContext? callContext = default)
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<SearchDocumentsResponse>(
            async cc => await Result<SearchDocumentsResponse>.FromAsync(async () =>
                await documentService.SearchDocumentsAsync(searchDocumentsRequest, cc)), callContext
        );
    }

    public async Task<Result<DeleteDocumentResponse>> DeleteDocumentAsync(IDocumentService documentService,
        DeleteDocumentRequest deleteDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<DeleteDocumentResponse>(
            async cc => await Result<DeleteDocumentResponse>.FromAsync(async () =>
                await documentService.DeleteDocumentAsync(deleteDocumentRequest, cc)), callContext
        );
    }

    public async Task<Result<ExistsDocumentResponse>> ExistsDocumentAsync(IDocumentService documentService,
        ExistsDocumentRequest existsDocumentRequest,
        CallContext? callContext = default)
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ExistsDocumentResponse>(
            async cc => await Result<ExistsDocumentResponse>.FromAsync(async () =>
                await documentService.ExistsDocumentAsync(existsDocumentRequest, cc)), callContext
        );
    }
}
