using System;
using System.Threading.Tasks;
using Persistify.Pipelines.Document.AddDocuments;
using Persistify.Pipelines.Document.GetDocument;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    private readonly AddDocumentsPipeline _addDocumentsPipeline;
    private readonly GetDocumentPipeline _getDocumentPipeline;

    public DocumentService(
        AddDocumentsPipeline addDocumentsPipeline,
        GetDocumentPipeline getDocumentPipeline
    )
    {
        _addDocumentsPipeline = addDocumentsPipeline;
        _getDocumentPipeline = getDocumentPipeline;
    }

    public async ValueTask<AddDocumentsResponse> Add(AddDocumentsRequest request, CallContext context)
    {
        return await _addDocumentsPipeline.ProcessAsync(request);
    }

    public ValueTask<GetDocumentResponse> Get(GetDocumentRequest request, CallContext context)
    {
        return _getDocumentPipeline.ProcessAsync(request);
    }

    public ValueTask<SearchDocumentsResponse> Search(SearchDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DeleteDocumentsResponse> Delete(DeleteDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
