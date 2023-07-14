using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.ListTemplate.Stages;

public class
    FetchTemplatesFromManagerStage : PipelineStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private const string StageName = "FetchTemplatesFromManager";
    private readonly ITemplateManager _templateManager;

    public FetchTemplatesFromManagerStage(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(ListTemplatesContext context)
    {
        var pagination = context.Request.Pagination ?? throw new PipelineException();
        var templates = (await _templateManager.GetAllAsync()).ToList();
        context.TotalCount = templates.Count;
        context.Templates = new List<Protos.Templates.Shared.Template>(Math.Min(templates.Count, pagination.PageSize));

        for (var i = 0; i < pagination.PageSize; i++)
        {
            var index = (pagination.PageNumber * pagination.PageSize) + i;
            if (index >= templates.Count)
            {
                break;
            }

            context.Templates.Add(templates[index]);
        }

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(ListTemplatesContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
