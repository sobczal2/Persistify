using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Common.Stages;

public class StaticValidationStage<TContext, TRequest, TResponse> : PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly IValidator<TRequest> _validator;
    private const string StageName = "StaticValidation";
    public override string Name => StageName;

    public StaticValidationStage(
        IValidator<TRequest> validator
    )
    {
        _validator = validator;
    }

    public override ValueTask<Result> ProcessAsync(TContext context)
    {
        var validationResult = _validator.Validate(context.Request);

        if (validationResult.IsFailure)
        {
            return ValueTask.FromResult(validationResult);
        }

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(TContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
