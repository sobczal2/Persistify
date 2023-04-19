using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents;

public class InsertTokensIntoIndexStoreMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly IIndexStore _indexStore;

    public InsertTokensIntoIndexStoreMiddleware(IIndexStore indexStore)
    {
        _indexStore = indexStore;
    }

    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        _indexStore.AddTokens(context.TypeDefinition?.Name ?? throw new InternalPipelineError(),
            context.Tokens ?? throw new InternalPipelineError(),
            context.DocumentId ?? throw new InternalPipelineError());

        context.PreviousPipelineStep = PipelineStep.IndexStore;
        
        context.SetResponse(new IndexDocumentResponseProto
        {
            DocumentId = context.DocumentId ?? throw new InternalPipelineError()
        });

        return Task.CompletedTask;
    }
}