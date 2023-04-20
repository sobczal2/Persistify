using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Grpc.Services;

public class DocumentService : DocumentsService.DocumentsServiceBase
{
    private readonly
        IPipelineOrchestrator<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>
        _indexDocumentPipelineOrchestrator;

    private readonly
        IPipelineOrchestrator<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
        _searchDocumentsPipelineOrchestrator;

    private readonly IPipelineOrchestrator<RemoveDocumentPipelineContext, RemoveDocumentRequestProto, RemoveDocumentResponseProto> _removeDocumentPipelineOrchestrator;

    public DocumentService(
        IPipelineOrchestrator<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>
            indexDocumentPipelineOrchestrator,
        IPipelineOrchestrator<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
            searchDocumentsPipelineOrchestrator,
        IPipelineOrchestrator<RemoveDocumentPipelineContext, RemoveDocumentRequestProto, RemoveDocumentResponseProto>
            removeDocumentPipelineOrchestrator)
    {
        _indexDocumentPipelineOrchestrator = indexDocumentPipelineOrchestrator;
        _searchDocumentsPipelineOrchestrator = searchDocumentsPipelineOrchestrator;
        _removeDocumentPipelineOrchestrator = removeDocumentPipelineOrchestrator;
    }

    public override async Task<IndexDocumentResponseProto> Index(IndexDocumentRequestProto request,
        ServerCallContext context)
    {
        var pipelineContext = new IndexDocumentPipelineContext(request);
        await _indexDocumentPipelineOrchestrator.ExecuteAsync(pipelineContext);
        return pipelineContext.Response;
    }

    public override async Task<SearchDocumentsResponseProto> Search(SearchDocumentsRequestProto request,
        ServerCallContext context)
    {
        var pipelineContext = new SearchDocumentsPipelineContext(request);
        await _searchDocumentsPipelineOrchestrator.ExecuteAsync(pipelineContext);
        return pipelineContext.Response;
    }

    public override async Task<RemoveDocumentResponseProto> Remove(RemoveDocumentRequestProto request, ServerCallContext context)
    {
        var pipelineContext = new RemoveDocumentPipelineContext(request);
        await _removeDocumentPipelineOrchestrator.ExecuteAsync(pipelineContext);
        return pipelineContext.Response;
    }
}