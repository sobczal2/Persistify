using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Document.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.DeleteTemplate.Stages;

public class
    DeleteTemplateFromDocumentManagerStage : PipelineStage<DeleteTemplateContext, DeleteTemplateRequest,
        DeleteTemplateResponse>
{
    private const string StageName = "DeleteTemplateFromDocumentManager";
    private readonly IDocumentManager _documentManager;
    private long? _currentDocumentIdOfDeletedTemplate;

    public DeleteTemplateFromDocumentManagerStage(
        IDocumentManager documentManager
    )
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(DeleteTemplateContext context)
    {
        _currentDocumentIdOfDeletedTemplate = await _documentManager.DeleteTemplateAsync(context.Request.TemplateName);

        return Result.Success;
    }

    public override async ValueTask<Result> RollbackAsync(DeleteTemplateContext context)
    {
        if (_currentDocumentIdOfDeletedTemplate is null)
        {
            return new RollbackFailedException();
        }

        await _documentManager.AddTemplateAsync(context.Request.TemplateName,
            _currentDocumentIdOfDeletedTemplate.Value);

        return Result.Success;
    }
}
