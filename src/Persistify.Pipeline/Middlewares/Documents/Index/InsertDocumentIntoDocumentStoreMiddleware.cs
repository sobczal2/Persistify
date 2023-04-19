using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents;

public class InsertDocumentIntoDocumentStoreMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext,
    IndexDocumentRequestProto, IndexDocumentResponseProto>
{
    private readonly IDocumentStore _documentStore;

    public InsertDocumentIntoDocumentStoreMiddleware(
        IDocumentStore documentStore
    )
    {
        _documentStore = documentStore;
    }

    public async Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        context.DocumentId = await _documentStore.AddAsync(context.Request.Data);

        context.PreviousPipelineStep = PipelineStep.DocumentStore;
    }
}