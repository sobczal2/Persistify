using System.Linq;
using System.Threading.Tasks;
using Persistify.Indexes.Common;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Documents.Remove;

[PipelineStep(PipelineStepType.Indexer)]
public class RemoveDocumentFromIndexersMiddleware : IPipelineMiddleware<RemoveDocumentPipelineContext,
    RemoveDocumentRequestProto, RemoveDocumentResponseProto>
{
    private readonly IIndexer<bool> _booleanIndexer;
    private readonly IIndexer<double> _numberIndexer;
    private readonly IIndexer<string> _textIndexer;

    public RemoveDocumentFromIndexersMiddleware(
        IIndexer<string> textIndexer,
        IIndexer<double> numberIndexer,
        IIndexer<bool> booleanIndexer)
    {
        _textIndexer = textIndexer;
        _numberIndexer = numberIndexer;
        _booleanIndexer = booleanIndexer;
    }

    public async Task InvokeAsync(RemoveDocumentPipelineContext context)
    {
        var removedIndexes = 0;

        var documentId = context.Request.DocumentId;
        var typeName = context.Request.TypeName;

        var removeTextTask = _textIndexer.RemoveAsync(documentId, typeName);
        var removeNumberTask = _numberIndexer.RemoveAsync(documentId, typeName);
        var removeBooleanTask = _booleanIndexer.RemoveAsync(documentId, typeName);

        await Task.WhenAll(removeTextTask, removeNumberTask, removeBooleanTask)
            .ContinueWith(task => { removedIndexes = task.Result.Sum(); });

        if (removedIndexes == 0)
            throw new ValidationException(new[] { ValidationFailures.DocumentNotFound });
    }
}