using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents.Remove;

public class RemoveDocumentFromDocumentStoreMiddleware : IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto, RemoveDocumentResponseProto>
{
    private readonly IDocumentStore _documentStore;

    public RemoveDocumentFromDocumentStoreMiddleware(
        IDocumentStore documentStore
        )
    {
        _documentStore = documentStore;
    }
    public async Task InvokeAsync(RemoveDocumentPipelineContext context)
    {
        await _documentStore.RemoveAsync(context.Request.DocumentId);
        
        context.SetResponse(new RemoveDocumentResponseProto());
    }
}