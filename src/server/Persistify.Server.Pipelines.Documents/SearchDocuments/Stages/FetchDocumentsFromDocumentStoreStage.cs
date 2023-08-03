using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

public class FetchDocumentsFromDocumentStoreStage : PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest
    , SearchDocumentsResponse>
{
    private readonly IDocumentManager _documentManager;
    private const string StageName = "FetchDocumentsFromDocumentStore";
    public override string Name => StageName;

    public FetchDocumentsFromDocumentStoreStage(
        ILogger<FetchDocumentsFromDocumentStoreStage> logger,
        IDocumentManager documentManager
        ) : base(logger)
    {
        _documentManager = documentManager;
    }

    public override async ValueTask ProcessAsync(SearchDocumentsPipelineContext context)
    {
        var documentIds = context.DocumentIds ?? throw new PipelineException();
        context.Documents = new List<Document>(documentIds.Count);

        foreach (var documentId in documentIds)
        {
            var document = await _documentManager.GetAsync(context.TemplateId, documentId);
            if (document == null)
            {
                throw new PipelineException();
            }
            context.Documents.Add(document);
        }
    }
}
