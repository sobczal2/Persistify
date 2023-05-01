using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators.Documents;

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
            throw new ValidationException(new[]
            {
                new ValidationFailure("TypeName", "Type not found")
                {
                    ErrorCode = DocumentErrorCodes.TypeNotFound
                }
            });
        
        context.TypeDefinition = _typeStore.Get(context.Request.TypeName);

        return Task.CompletedTask;
    }
}