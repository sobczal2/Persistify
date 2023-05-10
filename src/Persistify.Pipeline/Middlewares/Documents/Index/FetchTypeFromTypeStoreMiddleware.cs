using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

[PipelineStep(PipelineStepType.TypeStore)]
public class FetchTypeFromTypeStoreMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext,
    IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly ITypeStore _typeStore;

    public FetchTypeFromTypeStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        if (!_typeStore.Exists(context.Request.TypeName))
            throw new ValidationException(new[] { ValidationFailures.TypeNotFound });

        context.TypeDefinition = _typeStore.Get(context.Request.TypeName);

        return Task.CompletedTask;
    }
}