using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Documents;

public static class DocumentsClientExtensions
{
    public static async Task<Result<CreateDocumentResponse>> CreateDocumentAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        CreateDocumentRequest createDocumentRequest,
        CallContext? callContext = default
    )
    {
        return await Result<CreateDocumentResponse>.FromAsync(async () =>
        {
            var documentService = persistifyLowLevelClient.Documents.GetService();
            return await persistifyLowLevelClient.Documents.CreateDocumentAsync(
                documentService,
                createDocumentRequest,
                callContext
            );
        });
    }

    public static async Task<Result<GetDocumentResponse>> GetDocumentAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        GetDocumentRequest getDocumentRequest,
        CallContext? callContext = default
    )
    {
        return await Result<GetDocumentResponse>.FromAsync(async () =>
        {
            var documentService = persistifyLowLevelClient.Documents.GetService();
            return await persistifyLowLevelClient.Documents.GetDocumentAsync(
                documentService,
                getDocumentRequest,
                callContext
            );
        });
    }

    public static async Task<Result<SearchDocumentsResponse>> SearchDocumentsAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        SearchDocumentsRequest searchDocumentsRequest,
        CallContext? callContext = default
    )
    {
        return await Result<SearchDocumentsResponse>.FromAsync(async () =>
        {
            var documentService = persistifyLowLevelClient.Documents.GetService();
            return await persistifyLowLevelClient.Documents.SearchDocumentsAsync(
                documentService,
                searchDocumentsRequest,
                callContext
            );
        });
    }

    public static async Task<Result<DeleteDocumentResponse>> DeleteDocumentAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        DeleteDocumentRequest deleteDocumentRequest,
        CallContext? callContext = default
    )
    {
        return await Result<DeleteDocumentResponse>.FromAsync(async () =>
        {
            var documentService = persistifyLowLevelClient.Documents.GetService();
            return await persistifyLowLevelClient.Documents.DeleteDocumentAsync(
                documentService,
                deleteDocumentRequest,
                callContext
            );
        });
    }

    public static async Task<Result<ExistsDocumentResponse>> ExistsDocumentAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ExistsDocumentRequest existsDocumentRequest,
        CallContext? callContext = default
    )
    {
        return await Result<ExistsDocumentResponse>.FromAsync(async () =>
        {
            var documentService = persistifyLowLevelClient.Documents.GetService();
            return await persistifyLowLevelClient.Documents.ExistsDocumentAsync(
                documentService,
                existsDocumentRequest,
                callContext
            );
        });
    }
}
