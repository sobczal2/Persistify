using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Shared.Interfaces;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Shared.Stages;

public class
    FetchTemplateFromManagerStage<TContext, TRequest, TResponse> : PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>, IWithTemplate
    where TRequest : class
    where TResponse : class
{
    private const string StageName = "FetchTemplateFromManager";
    private readonly ITemplateManager _templateManager;

    public FetchTemplateFromManagerStage(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(TContext context)
    {
        var template = await _templateManager.GetAsync(context.TemplateName ?? throw new PipelineException());

        if (template is null)
        {
            return new ValidationException("TemplateName", $"Template {context.TemplateName} does not exist");
        }

        context.Template = template;

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(TContext context)
    {
        context.Template = null;

        return ValueTask.FromResult(Result.Success);
    }
}
