using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate;

public class CreateTemplatePipelineContext : IPipelineContext<CreateTemplateRequest, CreateTemplateResponse>
{
    public CreateTemplatePipelineContext(CreateTemplateRequest request)
    {
        Request = request;
    }

    public int? TemplateId { get; set; }
    public CreateTemplateRequest Request { get; set; }
    public CreateTemplateResponse? Response { get; set; }
}
