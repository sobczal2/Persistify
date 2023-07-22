using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.ListTemplates.Stages;

public class ApplyPaginationStage : PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private const string StageName = "ApplyPagination";
    public override string Name => StageName;
    public override ValueTask<Result> ProcessAsync(ListTemplatesPipelineContext context)
    {
        var templates = context.Templates ?? throw new PipelineException();
        var pagination = context.Request.Pagination ?? throw new PipelineException();

        var skip = pagination.PageNumber * pagination.PageSize;
        var take = pagination.PageSize;

        var templatesTemp = new List<Template>(take);

        for(var i = skip; i < skip + take; i++)
        {
            if (i >= templates.Count)
            {
                break;
            }

            templatesTemp.Add(templates[i]);
        }

        context.Templates = templatesTemp;
        context.TotalCount = templates.Count;

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(ListTemplatesPipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
