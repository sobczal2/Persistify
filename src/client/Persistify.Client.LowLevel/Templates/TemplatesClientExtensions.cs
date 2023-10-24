using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Templates;

public static class TemplatesClientExtensions
{
    public static async Task<Result<CreateTemplateResponse>> CreateTemplateAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        CreateTemplateRequest createTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyLowLevelClient.Templates.GetService();
        return await persistifyLowLevelClient.Templates.CreateTemplateAsync(templateService, createTemplateRequest,
            callContext);
    }

    public static async Task<Result<GetTemplateResponse>> GetTemplateAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyLowLevelClient.Templates.GetService();
        return await persistifyLowLevelClient.Templates.GetTemplateAsync(templateService, getTemplateRequest,
            callContext);
    }

    public static async Task<Result<ListTemplatesResponse>> ListTemplatesAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ListTemplatesRequest listTemplatesRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyLowLevelClient.Templates.GetService();
        return await persistifyLowLevelClient.Templates.ListTemplatesAsync(templateService, listTemplatesRequest,
            callContext);
    }

    public static async Task<Result<DeleteTemplateResponse>> DeleteTemplateAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        DeleteTemplateRequest deleteTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyLowLevelClient.Templates.GetService();
        return await persistifyLowLevelClient.Templates.DeleteTemplateAsync(templateService, deleteTemplateRequest,
            callContext);
    }

    public static async Task<Result<ExistsTemplateResponse>> ExistsTemplateAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ExistsTemplateRequest existsTemplateRequest,
        CallContext? callContext = default)
    {
        var templateService = persistifyLowLevelClient.Templates.GetService();
        return await persistifyLowLevelClient.Templates.ExistsTemplateAsync(templateService, existsTemplateRequest,
            callContext);
    }
}
