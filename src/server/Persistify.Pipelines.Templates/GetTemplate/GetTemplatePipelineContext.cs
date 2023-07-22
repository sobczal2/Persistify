using Persistify.Domain.Templates;
using Persistify.Pipelines.Common;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.GetTemplate;

public class GetTemplatePipelineContext : IPipelineContext<GetTemplateRequest, GetTemplateResponse>
{
    public GetTemplateRequest Request { get; set; }
    public GetTemplateResponse? Response { get; set; }
    public Template? Template { get; set; }

    public GetTemplatePipelineContext(GetTemplateRequest request)
    {
        Request = request;
    }
}
