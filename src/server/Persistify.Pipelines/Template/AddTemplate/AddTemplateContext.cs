using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.AddTemplate;

public class AddTemplateContext : IPipelineContext<AddTemplateRequest, AddTemplateResponse>
{
    public AddTemplateRequest Request { get; set; } = default!;
    public AddTemplateResponse? Response { get; set; }
}
