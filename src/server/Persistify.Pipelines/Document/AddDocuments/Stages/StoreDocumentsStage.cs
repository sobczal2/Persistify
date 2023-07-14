using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Document.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.AddDocuments.Stages;

public class StoreDocumentsStage : PipelineStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>
{
    private const string StageName = "StoreDocuments";
    private readonly IDocumentManager _documentManager;

    public StoreDocumentsStage(IDocumentManager documentManager)
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(AddDocumentsContext context)
    {
        var documents = context.Request.Documents;
        var templateName = context.Request.TemplateName;


        var tasks = new Task<long>[documents.Length];
        for (var i = 0; i < documents.Length; i++)
        {
            tasks[i] = _documentManager.AddAsync(templateName, documents[i]).AsTask();
        }

        context.DocumentIds = await Task.WhenAll(tasks);

        return Result.Success;
    }

    public override async ValueTask<Result> RollbackAsync(AddDocumentsContext context)
    {
        var templateName = context.Request.TemplateName;
        var documentIds = context.DocumentIds ?? throw new PipelineException();

        var tasks = new Task[documentIds.Length];
        for (var i = 0; i < documentIds.Length; i++)
        {
            tasks[i] = _documentManager.DeleteAsync(templateName, documentIds[i]).AsTask();
        }

        await Task.WhenAll(tasks);

        return Result.Success;
    }
}
