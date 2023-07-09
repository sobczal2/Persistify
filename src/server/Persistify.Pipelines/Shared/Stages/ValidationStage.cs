using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Pipelines.Common;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Shared.Stages;

public class ValidationStage<TContext, TRequest, TResponse> : PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly IValidator<TRequest> _validator;
    private const string StageName = "Validation";

    public ValidationStage(
        IValidator<TRequest> validator
    )
    {
        _validator = validator;
    }

    public override ValueTask<Result> ProcessAsync(TContext context)
    {
        var validationResult = _validator.Validate(context.Request);

        return validationResult.Match(
            _ => ValueTask.FromResult(Result.Success),
            x => ValueTask.FromResult<Result>(x)
        );
    }

    public override ValueTask<Result> RollbackAsync(TContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }

    public override string Name => StageName;
}
