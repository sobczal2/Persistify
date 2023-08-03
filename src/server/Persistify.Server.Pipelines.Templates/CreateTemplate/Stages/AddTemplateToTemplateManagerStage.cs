using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

public class AddTemplateToTemplateManagerStage : PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest,
    CreateTemplateResponse>
{
    public const string StageName = "AddTemplateToTemplateManager";
    private readonly ITemplateManager _templateManager;

    public AddTemplateToTemplateManagerStage(
        ILogger<AddTemplateToTemplateManagerStage> logger,
        ITemplateManager templateManager
    ) : base(logger)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask ProcessAsync(CreateTemplatePipelineContext context)
    {
        var template = new Template
        {
            Name = context.Request.TemplateName,
            TextFields = context.Request.TextFields,
            NumberFields = context.Request.NumberFields,
            BoolFields = context.Request.BoolFields
        };

        await _templateManager.AddAsync(template);


        context.Template = template;
    }
}
