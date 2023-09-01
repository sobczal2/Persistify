using System;
using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    public TemplateService(
    )
    {
    }

    public ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request)
    {
        throw new NotImplementedException();
    }
}
