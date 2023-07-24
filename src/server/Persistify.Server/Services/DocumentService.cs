using System;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Documents.GetDocument;
using Persistify.Server.Pipelines.Documents.IndexDocument;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly GetDocumentPipeline _getDocumentPipeline;
    private readonly IndexDocumentPipeline _indexDocumentPipeline;

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
