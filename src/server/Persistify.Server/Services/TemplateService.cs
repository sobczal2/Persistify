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

    public TemplateService(AddTemplatePipeline addTemplatePipeline)
    {
        _addTemplatePipeline = addTemplatePipeline;
    }
    public async ValueTask<AddTemplateResponse> Add(AddTemplateRequest request, CallContext context)
    {
        return await _addTemplatePipeline.ProcessAsync(request);
    }

    public ValueTask<ListTemplatesResponse> List(ListTemplatesRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<DeleteTemplateResponse> Delete(DeleteTemplateRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }
}
