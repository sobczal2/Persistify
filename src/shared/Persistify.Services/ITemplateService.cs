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
    ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request);

    [OperationContract]
    ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request);

    [OperationContract]
    ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request);

    [OperationContract]
    ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request);
}
