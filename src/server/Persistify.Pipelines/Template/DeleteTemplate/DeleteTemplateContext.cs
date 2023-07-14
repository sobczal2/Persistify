using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.DeleteTemplate;

public class DeleteTemplateContext : IPipelineContext<DeleteTemplateRequest, DeleteTemplateResponse>
{
    public Protos.Templates.Shared.Template? DeletedTemplate { get; set; }
    public DeleteTemplateRequest Request { get; set; } = default!;
    public DeleteTemplateResponse? Response { get; set; }
}
