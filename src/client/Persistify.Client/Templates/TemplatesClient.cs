using Persistify.Client.Core;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Templates;

public class TemplatesClient : SubClient<ITemplateService>, ITemplatesClient
{
    internal TemplatesClient(PersistifyClient persistifyClient) : base(persistifyClient)
    {
    }

    public async Task<CreateTemplateResponse> CreateTemplateAsync(ITemplateService templateService,
        CreateTemplateRequest createTemplateRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<CreateTemplateResponse>(
            async cc => await templateService.CreateTemplateAsync(createTemplateRequest, cc), callContext
        );
    }

    public async Task<GetTemplateResponse> GetTemplateAsync(ITemplateService templateService,
        GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<GetTemplateResponse>(
            async cc => await templateService.GetTemplateAsync(getTemplateRequest, cc), callContext
        );
    }

    public async Task<ListTemplatesResponse> ListTemplatesAsync(ITemplateService templateService,
        ListTemplatesRequest listTemplatesRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<ListTemplatesResponse>(
            async cc => await templateService.ListTemplatesAsync(listTemplatesRequest, cc), callContext
        );
    }

    public async Task<DeleteTemplateResponse> DeleteTemplateAsync(ITemplateService templateService,
        DeleteTemplateRequest deleteTemplateRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<DeleteTemplateResponse>(
            async cc => await templateService.DeleteTemplateAsync(deleteTemplateRequest, cc), callContext
        );
    }

    public async Task<ExistsTemplateResponse> ExistsTemplateAsync(ITemplateService templateService,
        ExistsTemplateRequest existsTemplateRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<ExistsTemplateResponse>(
            async cc => await templateService.ExistsTemplateAsync(existsTemplateRequest, cc), callContext
        );
    }
}
