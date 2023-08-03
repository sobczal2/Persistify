using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common.Contexts;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Common.Stages;

public class
    FetchTemplateFromTemplateManagerStage<TContext, TRequest,
        TResponse> : PipelineStage<TContext, TRequest, TResponse>
    where TContext : class,
    IPipelineContext<TRequest, TResponse>,
    IContextWithTemplate
    where TRequest : class
    where TResponse : class
{
    private const string StageName = "FetchTemplatesFromTemplateManager";
    private readonly ITemplateManager _templateManager;

    public FetchTemplateFromTemplateManagerStage(
        ILogger<FetchTemplateFromTemplateManagerStage<TContext, TRequest, TResponse>> logger,
        ITemplateManager templateManager
    ) :
        base(logger)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;
    public override async ValueTask ProcessAsync(TContext context)
    {
        var template = await _templateManager.GetAsync(context.TemplateId);
        if (template is null)
        {
            throw new ValidationException("GetTemplateRequest.TemplateId",
                "Template not found");
        }

        context.Template = template;
    }
}
