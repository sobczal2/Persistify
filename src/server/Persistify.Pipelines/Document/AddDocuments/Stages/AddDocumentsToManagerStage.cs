using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Common.Abstracts;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.AddDocuments.Stages;

public class
    AddDocumentsToManagerStage<TManager> : PipelineStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>
    where TManager : IAddManager, IDeleteManager
{
    public static string StageName = $"AddDocumentsTo{typeof(TManager).Name}Manager";
    private readonly TManager _manager;

    public AddDocumentsToManagerStage(
        TManager manager)
    {
        _manager = manager;
    }

    public override string Name => StageName;

    public override ValueTask<Result> ProcessAsync(AddDocumentsContext context)
    {
        var documents = context.Request.Documents;
        var templateName = context.Request.TemplateName;
        var documentIds = context.DocumentIds ?? throw new PipelineException();

        for (var i = 0; i < documents.Length; i++)
        {
            _manager.Add(templateName, documents[i], documentIds[i]);
        }

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(AddDocumentsContext context)
    {
        var templateName = context.Request.TemplateName;
        var documentIds = context.DocumentIds ?? throw new PipelineException();

        for (var i = 0; i < documentIds.Length; i++)
        {
            _manager.Delete(templateName, documentIds[i]);
        }

        return ValueTask.FromResult(Result.Success);
    }
}
