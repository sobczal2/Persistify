using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Common;

[PipelineStep(PipelineStepType.StaticValidation)]
public class
    RequestProtoValidationMiddleware<TContext, TRequest, TResponse> : IPipelineMiddleware<TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly IValidator<TRequest> _validator;

    public RequestProtoValidationMiddleware(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public Task InvokeAsync(TContext context)
    {
        _validator.ValidateAndThrow(context.Request);

        return Task.CompletedTask;
    }
}