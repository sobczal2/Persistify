using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Objects;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.RequestValidators.Objects;
using Persistify.RequestValidators.Types;
using Persistify.Stores.Types;

namespace Persistify.Pipeline.Middlewares.Objects;

public class FetchTypeFromStoreMiddleware : IPipelineMiddleware<IndexObjectPipelineContext, IndexObjectRequestProto,
    IndexObjectResponseProto>
{
    private readonly ITypeStore _typeStore;

    public FetchTypeFromStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(IndexObjectPipelineContext context)
    {
        try
        {
            context.TypeDefinition = _typeStore.Get(context.Request.TypeName);
        }
        catch (TypeNotFoundException)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("TypeName", "Type not found")
                {
                    ErrorCode = ObjectErrorCodes.TypeNotFound
                }
            });
        }
        
        context.PreviousPipelineStep = PipelineStep.TypeStore;
        return Task.CompletedTask;
    }
}