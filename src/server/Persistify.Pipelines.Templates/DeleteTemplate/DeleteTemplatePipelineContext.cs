using Persistify.Domain.Templates;
using Persistify.Pipelines.Common;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.DeleteTemplate;

public class DeleteTemplatePipelineContext : IPipelineContext<DeleteTemplateRequest, DeleteTemplateResponse>
{
    public DeleteTemplateRequest Request { get; set; }
    public DeleteTemplateResponse? Response { get; set; }
    public Template? Template { get; set; }

    public DeleteTemplatePipelineContext(DeleteTemplateRequest request)
    {
        Request = request;
    }
}
