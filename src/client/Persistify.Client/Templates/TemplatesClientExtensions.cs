using Persistify.Client.Core;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using ProtoBuf.Grpc;

namespace Persistify.Client.Templates;

public static class TemplatesClientExtensions
{
    public static async Task<CreateTemplateResponse> CreateTemplateAsync(this IPersistifyClient persistifyClient,
        CreateTemplateRequest createTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyClient.Templates.GetService();
        return await persistifyClient.Templates.CreateTemplateAsync(templateService, createTemplateRequest,
            callContext);
    }

    public static async Task<GetTemplateResponse> GetTemplateAsync(this IPersistifyClient persistifyClient,
        GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyClient.Templates.GetService();
        return await persistifyClient.Templates.GetTemplateAsync(templateService, getTemplateRequest, callContext);
    }

    public static async Task<ListTemplatesResponse> ListTemplatesAsync(this IPersistifyClient persistifyClient,
        ListTemplatesRequest listTemplatesRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyClient.Templates.GetService();
        return await persistifyClient.Templates.ListTemplatesAsync(templateService, listTemplatesRequest, callContext);
    }

    public static async Task<DeleteTemplateResponse> DeleteTemplateAsync(this IPersistifyClient persistifyClient,
        DeleteTemplateRequest deleteTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyClient.Templates.GetService();
        return await persistifyClient.Templates.DeleteTemplateAsync(templateService, deleteTemplateRequest,
            callContext);
    }

    public static async Task<ExistsTemplateResponse> ExistsTemplateAsync(this IPersistifyClient persistifyClient,
        ExistsTemplateRequest existsTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyClient.Templates.GetService();
        return await persistifyClient.Templates.ExistsTemplateAsync(templateService, existsTemplateRequest,
            callContext);
    }
}
