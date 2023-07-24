using System.Collections.Generic;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.ListTemplates;

public class ListTemplatesPipelineContext : IPipelineContext<ListTemplatesRequest, ListTemplatesResponse>
{
    public ListTemplatesPipelineContext(ListTemplatesRequest request)
    {
        Request = request;
    }

    public IEnumerable<Template>? Templates { get; set; }
    public int TotalCount { get; set; }
    public ListTemplatesRequest Request { get; set; }
    public ListTemplatesResponse? Response { get; set; }
}
