using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Templates;

public interface ITemplatesClient
{
    ITemplateService GetService();

    Task<CreateTemplateResponse> CreateTemplateAsync(ITemplateService templateService,
        CreateTemplateRequest createTemplateRequest, CallContext? callContext = default);

    Task<GetTemplateResponse> GetTemplateAsync(ITemplateService templateService, GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default);

    Task<ListTemplatesResponse> ListTemplatesAsync(ITemplateService templateService,
        ListTemplatesRequest listTemplatesRequest, CallContext? callContext = default);

    Task<DeleteTemplateResponse> DeleteTemplateAsync(ITemplateService templateService,
        DeleteTemplateRequest deleteTemplateRequest, CallContext? callContext = default);

    Task<ExistsTemplateResponse> ExistsTemplateAsync(ITemplateService templateService,
        ExistsTemplateRequest existsTemplateRequest, CallContext? callContext = default);
}
