using System.Threading.Tasks;
using Persistify.Pipelines.Template.Pipelines;
using Persistify.Protos.Templates;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly AddTemplatePipeline _addTemplatePipeline;
    private readonly ListTemplatesPipeline _listTemplatesPipeline;

    public TemplateService(
        AddTemplatePipeline addTemplatePipeline,
        ListTemplatesPipeline listTemplatesPipeline
        )
    {
        _addTemplatePipeline = addTemplatePipeline;
        _listTemplatesPipeline = listTemplatesPipeline;
    }
    public async ValueTask<AddTemplateResponse> Add(AddTemplateRequest request, CallContext context)
    {
        return await _addTemplatePipeline.ProcessAsync(request);
    }

    public async ValueTask<ListTemplatesResponse> List(ListTemplatesRequest request, CallContext context)
    {
        return await _listTemplatesPipeline.ProcessAsync(request);
    }

    public ValueTask<DeleteTemplateResponse> Delete(DeleteTemplateRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }
}
