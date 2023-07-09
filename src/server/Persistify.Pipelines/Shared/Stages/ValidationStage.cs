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

    public ValidationStage(
        IValidator<TRequest> validator
    )
    {
        _validator = validator;
    }

    public override ValueTask<Result> Process(TContext context)
    {
        var validationResult = _validator.Validate(context.Request);

        return validationResult.Match(
            _ => ValueTask.FromResult(Result.Success),
            x => ValueTask.FromResult<Result>(x)
        );
    }
}
