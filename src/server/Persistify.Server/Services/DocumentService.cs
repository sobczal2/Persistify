using System;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Documents.GetDocument;
using Persistify.Server.Pipelines.Documents.IndexDocument;
using Persistify.Server.Pipelines.Documents.SearchDocuments;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly GetDocumentPipeline _getDocumentPipeline;
    private readonly SearchDocumentsPipeline _searchDocumentsPipeline;
    private readonly IndexDocumentPipeline _indexDocumentPipeline;

    public DocumentService(
        IndexDocumentPipeline indexDocumentPipeline,
        GetDocumentPipeline getDocumentPipeline,
        SearchDocumentsPipeline searchDocumentsPipeline
    )
    {
        _indexDocumentPipeline = indexDocumentPipeline;
        _getDocumentPipeline = getDocumentPipeline;
        _searchDocumentsPipeline = searchDocumentsPipeline;
    }

    public async ValueTask<IndexDocumentResponse> IndexDocumentAsync(IndexDocumentRequest request, CallContext context)
    {
        return await _indexDocumentPipeline.ProcessAsync(request);
    }

    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context)
    {
        return await _getDocumentPipeline.ProcessAsync(request);
    }

    public async ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext context)
    {
        return await _searchDocumentsPipeline.ProcessAsync(request);
    }

    public ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
