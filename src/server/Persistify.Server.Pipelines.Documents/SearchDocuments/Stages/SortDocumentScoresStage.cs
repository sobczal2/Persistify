using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

public class SortDocumentScoresStage : PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
    SearchDocumentsResponse>
{
    private const string StageName = "SortDocumentScores";
    public override string Name => StageName;

    public SortDocumentScoresStage(ILogger<SortDocumentScoresStage> logger) : base(logger)
    {
    }

    public override ValueTask<Result> ProcessAsync(SearchDocumentsPipelineContext context)
    {
        var documentScores = context.DocumentScores ?? throw new PipelineException();
        documentScores.Sort((x, y) => y.Score.CompareTo(x.Score));

        return ValueTask.FromResult(Result.Success);
    }

    public override async ValueTask<Result> RollbackAsync(SearchDocumentsPipelineContext context)
    {
        return await Task.FromResult(Result.Success);
    }
}
