using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Templates.DeleteTemplate.Stages;

public class RemoveTemplateFromTypeManagersStage : PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest,
    DeleteTemplateResponse>
{
    private readonly IEnumerable<ITypeManager> _typeManagers;
    private const string StageName = "RemoveTemplateFromTypeManagers";
    public override string Name => StageName;

    public RemoveTemplateFromTypeManagersStage(
        ILogger<RemoveTemplateFromTypeManagersStage> logger,
        IEnumerable<ITypeManager> typeManagers
    ) : base(logger)
    {
        _typeManagers = typeManagers;
    }

    public override async ValueTask ProcessAsync(DeleteTemplatePipelineContext context)
    {
        var template = context.Template ?? throw new PipelineException();

        foreach (var typeManager in _typeManagers)
        {
            await typeManager.RemoveForTemplate(template);
        }
    }
}
