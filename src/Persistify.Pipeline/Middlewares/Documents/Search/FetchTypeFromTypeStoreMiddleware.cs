using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.TypeStore)]
public class FetchTypeFromTypeStoreMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly ITypeStore _typeStore;

    public FetchTypeFromTypeStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        if (!_typeStore.Exists(context.Request.TypeName))
            throw new ValidationException(new[] { ValidationFailures.TypeNotFound });

        context.TypeDefinition = _typeStore.Get(context.Request.TypeName);

        return Task.CompletedTask;
    }
}