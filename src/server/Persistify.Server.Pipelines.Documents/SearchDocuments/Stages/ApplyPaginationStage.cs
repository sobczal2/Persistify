using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

public class ApplyPaginationStage : PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
    SearchDocumentsResponse>
{
    private const string StageName = "ApplyPagination";
    public override string Name => StageName;

    public ApplyPaginationStage(
        ILogger<ApplyPaginationStage> logger
    ) : base(
        logger
    )
    {
    }

    public override ValueTask ProcessAsync(SearchDocumentsPipelineContext context)
    {
        var documentScores = context.DocumentScores ?? throw new PipelineException();
        var pagination = context.Request.Pagination;

        var take = pagination.PageSize;
        var skip = pagination.PageSize * pagination.PageNumber;

        context.DocumentIds = new List<int>(take);

        for(var i = skip; i < skip + take; i++)
        {
            if (i >= documentScores.Count)
            {
                break;
            }

            context.DocumentIds.Add(documentScores[i].DocumentId);
        }

        context.TotalCount = documentScores.Count;
        return ValueTask.CompletedTask;
    }
}
