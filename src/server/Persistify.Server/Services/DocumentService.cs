using System;
using System.Threading.Tasks;
using Persistify.Pipelines.Documents.GetDocument;
using Persistify.Pipelines.Documents.IndexDocument;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly IndexDocumentPipeline _indexDocumentPipeline;
    private readonly GetDocumentPipeline _getDocumentPipeline;

    public DocumentService(
        IndexDocumentPipeline indexDocumentPipeline,
        GetDocumentPipeline getDocumentPipeline
        )
    {
        _indexDocumentPipeline = indexDocumentPipeline;
        _getDocumentPipeline = getDocumentPipeline;
    }
    public ValueTask<IndexDocumentResponse> IndexDocumentAsync(IndexDocumentRequest request, CallContext context)
    {
        return _indexDocumentPipeline.ProcessAsync(request);
    }

    public ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context)
    {
        return _getDocumentPipeline.ProcessAsync(request);
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
