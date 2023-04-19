using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators.Types;
using Persistify.Stores.Types;

namespace Persistify.Pipeline.Middlewares.Types;

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
            throw new ValidationException(new[]
            {
                new ValidationFailure("TypeName", "Type already exists")
                {
                    ErrorCode = TypeErrorCodes.NameDuplicate
                }
            });

        _typeStore.Create(context.Request.TypeDefinition);

        context.SetResponse(new CreateTypeResponseProto());

        return Task.CompletedTask;
    }
}