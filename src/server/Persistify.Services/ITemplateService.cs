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
    ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CallContext context);

    [OperationContract]
    ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext context);

    [OperationContract]
    ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext context);

    [OperationContract]
    ValueTask<DeleteTemplateRequest> DeleteTemplateAsync(DeleteTemplateRequest request, CallContext context);
}
