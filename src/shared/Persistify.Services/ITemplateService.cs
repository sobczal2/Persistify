using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface ITemplateService
{
    [OperationContract]
    ValueTask<CreateTemplateResponse> CreateTemplateAsync(
        CreateTemplateRequest request,
        CallContext callContext
    );

    [OperationContract]
    ValueTask<GetTemplateResponse> GetTemplateAsync(
        GetTemplateRequest request,
        CallContext callContext
    );

    [OperationContract]
    ValueTask<ListTemplatesResponse> ListTemplatesAsync(
        ListTemplatesRequest request,
        CallContext callContext
    );

    [OperationContract]
    ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(
        DeleteTemplateRequest request,
        CallContext callContext
    );

    [OperationContract]
    ValueTask<ExistsTemplateResponse> ExistsTemplateAsync(
        ExistsTemplateRequest request,
        CallContext callContext
    );
}
