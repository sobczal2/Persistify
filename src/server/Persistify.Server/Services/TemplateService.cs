using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    public ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<DeleteTemplateRequest> DeleteTemplateAsync(DeleteTemplateRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }
}
