using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Pipelines.Common;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.ListTemplates;

public class ListTemplatesPipelineContext : IPipelineContext<ListTemplatesRequest, ListTemplatesResponse>
{
    public ListTemplatesRequest Request { get; set; }
    public ListTemplatesResponse? Response { get; set; }
    public List<Template>? Templates { get; set; }
    public int TotalCount { get; set; }

    public ListTemplatesPipelineContext(ListTemplatesRequest request)
    {
        Request = request;
    }
}
