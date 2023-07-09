using System.Collections.Generic;
using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.ListTemplate;

public class ListTemplatesContext : IPipelineContext<ListTemplatesRequest, ListTemplatesResponse>
{
    public List<Protos.Templates.Shared.Template>? Templates { get; set; }
    public long? TotalCount { get; set; }
    public ListTemplatesRequest Request { get; set; } = default!;
    public ListTemplatesResponse? Response { get; set; }
}
