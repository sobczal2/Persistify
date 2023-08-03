using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Templates.CreateTemplate;
using Persistify.Server.Pipelines.Templates.DeleteTemplate;
using Persistify.Server.Pipelines.Templates.GetTemplate;
using Persistify.Server.Pipelines.Templates.ListTemplates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly CreateTemplatePipeline _createTemplatePipeline;
    private readonly DeleteTemplatePipeline _deleteTemplatePipeline;
    private readonly GetTemplatePipeline _getTemplatePipeline;
    private readonly ListTemplatesPipeline _listTemplatesPipeline;

    public TemplateService(
        CreateTemplatePipeline createTemplatePipeline,
        GetTemplatePipeline getTemplatePipeline,
        ListTemplatesPipeline listTemplatesPipeline,
        DeleteTemplatePipeline deleteTemplatePipeline
    )
    {
        _createTemplatePipeline = createTemplatePipeline;
        _getTemplatePipeline = getTemplatePipeline;
        _listTemplatesPipeline = listTemplatesPipeline;
        _deleteTemplatePipeline = deleteTemplatePipeline;
    }

    public async ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request)
    {
        return await _createTemplatePipeline.ProcessAsync(request);
    }

    public async ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request)
    {
        return await _getTemplatePipeline.ProcessAsync(request);
    }

    public async ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request)
    {
        return await _listTemplatesPipeline.ProcessAsync(request);
    }

    public async ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request)
    {
        return await _deleteTemplatePipeline.ProcessAsync(request);
    }
}
