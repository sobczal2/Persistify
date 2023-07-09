using System.Collections.Generic;
using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.Contexts;

public class ListTemplatesContext : IPipelineContext<ListTemplatesRequest, ListTemplatesResponse>
{
    public ListTemplatesRequest Request { get; set; } = default!;
    public ListTemplatesResponse? Response { get; set; }
    public List<Protos.Templates.Shared.Template>? Templates { get; set; }
    public long? TotalCount { get; set; }
}
