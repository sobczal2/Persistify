using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Templates;

public class TemplatesClient : SubClient<ITemplateService>, ITemplatesClient
{
    internal TemplatesClient(PersistifyLowLevelClient persistifyLowLevelClient)
        : base(persistifyLowLevelClient) { }

    public async Task<Result<CreateTemplateResponse>> CreateTemplateAsync(
        ITemplateService templateService,
        CreateTemplateRequest createTemplateRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<CreateTemplateResponse>(
            async cc =>
                await Result<CreateTemplateResponse>.FromAsync(
                    async () => await templateService.CreateTemplateAsync(createTemplateRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<GetTemplateResponse>> GetTemplateAsync(
        ITemplateService templateService,
        GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<GetTemplateResponse>(
            async cc =>
                await Result<GetTemplateResponse>.FromAsync(
                    async () => await templateService.GetTemplateAsync(getTemplateRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<ListTemplatesResponse>> ListTemplatesAsync(
        ITemplateService templateService,
        ListTemplatesRequest listTemplatesRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ListTemplatesResponse>(
            async cc =>
                await Result<ListTemplatesResponse>.FromAsync(
                    async () => await templateService.ListTemplatesAsync(listTemplatesRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<DeleteTemplateResponse>> DeleteTemplateAsync(
        ITemplateService templateService,
        DeleteTemplateRequest deleteTemplateRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<DeleteTemplateResponse>(
            async cc =>
                await Result<DeleteTemplateResponse>.FromAsync(
                    async () => await templateService.DeleteTemplateAsync(deleteTemplateRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<ExistsTemplateResponse>> ExistsTemplateAsync(
        ITemplateService templateService,
        ExistsTemplateRequest existsTemplateRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ExistsTemplateResponse>(
            async cc =>
                await Result<ExistsTemplateResponse>.FromAsync(
                    async () => await templateService.ExistsTemplateAsync(existsTemplateRequest, cc)
                ),
            callContext
        );
    }
}
