using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators.Common;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Types.Create;

[PipelineStep(PipelineStepType.TypeStore)]
public class AddTypeToTypeStoreMiddleware : IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto,
    CreateTypeResponseProto>
{
    private readonly ITypeStore _typeStore;

    public AddTypeToTypeStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(CreateTypePipelineContext context)
    {
        if (_typeStore.Exists(context.Request.TypeDefinition.Name))
            throw new ValidationException(new[] { ValidationFailures.TypeAlreadyExists });

        _typeStore.Create(context.Request.TypeDefinition);

        context.SetResponse(new CreateTypeResponseProto());

        return Task.CompletedTask;
    }
}