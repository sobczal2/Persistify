using System;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Documents.AddDocument;
using Persistify.Server.Pipelines.Documents.GetDocument;
using Persistify.Server.Pipelines.Documents.SearchDocuments;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly GetDocumentPipeline _getDocumentPipeline;
    private readonly SearchDocumentsPipeline _searchDocumentsPipeline;
    private readonly AddDocumentPipeline _addDocumentPipeline;

    public DocumentService(
        AddDocumentPipeline addDocumentPipeline,
        GetDocumentPipeline getDocumentPipeline,
        SearchDocumentsPipeline searchDocumentsPipeline
    )
    {
        _addDocumentPipeline = addDocumentPipeline;
        _getDocumentPipeline = getDocumentPipeline;
        _searchDocumentsPipeline = searchDocumentsPipeline;
    }

    public async ValueTask<AddDocumentResponse> AddDocumentAsync(AddDocumentRequest request)
    {
        return await _addDocumentPipeline.ProcessAsync(request);
    }

    public async ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request)
    {
        return await _getDocumentPipeline.ProcessAsync(request);
    }

    public async ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request)
    {
        return await _searchDocumentsPipeline.ProcessAsync(request);
    }

    public ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request)
    {
        throw new NotImplementedException();
    }
}
