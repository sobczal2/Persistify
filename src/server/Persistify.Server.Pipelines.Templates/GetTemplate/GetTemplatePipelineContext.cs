using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.GetTemplate;

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
