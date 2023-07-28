using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Contexts;

namespace Persistify.Server.Pipelines.Templates.GetTemplate;

public class GetTemplatePipelineContext : IPipelineContext<GetTemplateRequest, GetTemplateResponse>, IContextWithTemplate
{
    public GetTemplatePipelineContext(GetTemplateRequest request, int templateId)
    {
        Request = request;
        TemplateId = templateId;
    }

    public int TemplateId { get; set; }
    public Template? Template { get; set; }
    public GetTemplateRequest Request { get; set; }
    public GetTemplateResponse? Response { get; set; }
}
