using System;
using System.Threading.Tasks;
using Persistify.Pipelines.Templates.CreateTemplates;
using Persistify.Pipelines.Templates.GetTemplates;
using Persistify.Pipelines.Templates.ListTemplates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly CreateTemplatePipeline _createTemplatePipeline;
    private readonly GetTemplatePipeline _getTemplatePipeline;
    private readonly ListTemplatesPipeline _listTemplatesPipeline;

    public TemplateService(
        CreateTemplatePipeline createTemplatePipeline,
        GetTemplatePipeline getTemplatePipeline,
        ListTemplatesPipeline listTemplatesPipeline
        )
    {
        _createTemplatePipeline = createTemplatePipeline;
        _getTemplatePipeline = getTemplatePipeline;
        _listTemplatesPipeline = listTemplatesPipeline;
    }
    public ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CallContext context)
    {
        return _createTemplatePipeline.ProcessAsync(request);
    }

    public ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext context)
    {
        return _getTemplatePipeline.ProcessAsync(request);
    }

    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext context)
    {
        return _listTemplatesPipeline.ProcessAsync(request);
    }

    public ValueTask<DeleteTemplateRequest> DeleteTemplateAsync(DeleteTemplateRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
