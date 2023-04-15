using System.Threading.Tasks;
using FluentValidation;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Middlewares.Abstractions;

namespace Persistify.Pipeline.Middlewares.Common;

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
        context.PreviousPipelineStep = PipelineStep.Validation;
        return Task.CompletedTask;
    }
}