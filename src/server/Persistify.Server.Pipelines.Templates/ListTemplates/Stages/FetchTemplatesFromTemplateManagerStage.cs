using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.ListTemplates.Stages;

public class
    FetchTemplatesFromTemplateManagerStage : PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest,
        ListTemplatesResponse>
{
    public const string StageName = "FetchTemplatesFromTemplateManager";
    private readonly ITemplateManager _templateManager;

    public FetchTemplatesFromTemplateManagerStage(
        ILogger<FetchTemplatesFromTemplateManagerStage> logger,
        ITemplateManager templateManager
    ) : base(logger)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override ValueTask ProcessAsync(ListTemplatesPipelineContext context)
    {
        // TODO: Implement

        return ValueTask.CompletedTask;
    }
}
