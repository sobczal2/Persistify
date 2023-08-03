using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Common.Stages;

public class StaticValidationStage<TContext, TRequest, TResponse> : PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private const string StageName = "StaticValidation";
    private readonly IValidator<TRequest> _validator;

    public StaticValidationStage(
        ILogger<StaticValidationStage<TContext, TRequest, TResponse>> logger,
        IValidator<TRequest> validator
    ) : base(logger)
    {
        _validator = validator;
    }

    public override string Name => StageName;

    public override ValueTask ProcessAsync(TContext context)
    {
        var validationResult = _validator.Validate(context.Request);

        if (validationResult.IsFailure)
        {
            throw validationResult.Exception;
        }

        return ValueTask.CompletedTask;
    }
}
