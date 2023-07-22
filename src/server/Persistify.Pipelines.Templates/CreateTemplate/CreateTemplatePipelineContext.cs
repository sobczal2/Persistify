using Persistify.Pipelines.Common;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.CreateTemplate;

public class CreateTemplatePipelineContext : IPipelineContext<CreateTemplateRequest, CreateTemplateResponse>
{
    public CreateTemplateRequest Request { get; set; }
    public CreateTemplateResponse? Response { get; set; }
    public int? TemplateId { get; set; }

    public CreateTemplatePipelineContext(CreateTemplateRequest request)
    {
        Request = request;
    }
}
