using System.Threading.Tasks;
using Persistify.Indexes.Common;
using Persistify.Indexes.Text;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

[PipelineStep(PipelineStepType.IndexStore)]
public class InsertTokensIntoIndexersMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext,
    IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly IIndexer<string> _textIndexer;
    private readonly IIndexer<double> _numberIndexer;
    private readonly IIndexer<bool> _booleanIndexer;

    public InsertTokensIntoIndexersMiddleware(
        IIndexer<string> textIndexer,
        IIndexer<double> numberIndexer,
        IIndexer<bool> booleanIndexer
        )
    {
        _textIndexer = textIndexer;
        _numberIndexer = numberIndexer;
        _booleanIndexer = booleanIndexer;
    }

    public async Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        var documentId = context.DocumentId ?? throw new InternalPipelineError();
        var textTokens = context.TextTokens ?? throw new InternalPipelineError();
        var numberTokens = context.NumberTokens ?? throw new InternalPipelineError();
        var booleanTokens = context.BooleanTokens ?? throw new InternalPipelineError();
        var typeDefinition = context.TypeDefinition ?? throw new InternalPipelineError();
        
        var indexTextTask = _textIndexer.IndexAsync(documentId, textTokens, typeDefinition.Name);
        var indexNumberTask = _numberIndexer.IndexAsync(documentId, numberTokens, typeDefinition.Name);
        var indexBooleanTask = _booleanIndexer.IndexAsync(documentId, booleanTokens, typeDefinition.Name);
        
        await Task.WhenAll(indexTextTask, indexNumberTask, indexBooleanTask);

        context.SetResponse(new IndexDocumentResponseProto
        {
            DocumentId = context.DocumentId ?? throw new InternalPipelineError()
        });
    }
}