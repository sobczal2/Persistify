using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Templates;

public interface ITemplatesClient
{
    ITemplateService GetService();

    Task<Result<CreateTemplateResponse>> CreateTemplateAsync(
        ITemplateService templateService,
        CreateTemplateRequest createTemplateRequest,
        CallContext? callContext = default
    );

    Task<Result<GetTemplateResponse>> GetTemplateAsync(
        ITemplateService templateService,
        GetTemplateRequest getTemplateRequest,
        CallContext? callContext = default
    );

    Task<Result<ListTemplatesResponse>> ListTemplatesAsync(
        ITemplateService templateService,
        ListTemplatesRequest listTemplatesRequest,
        CallContext? callContext = default
    );

    Task<Result<DeleteTemplateResponse>> DeleteTemplateAsync(
        ITemplateService templateService,
        DeleteTemplateRequest deleteTemplateRequest,
        CallContext? callContext = default
    );

    Task<Result<ExistsTemplateResponse>> ExistsTemplateAsync(
        ITemplateService templateService,
        ExistsTemplateRequest existsTemplateRequest,
        CallContext? callContext = default
    );
}
