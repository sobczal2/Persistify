using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;
using ProtoBuf.Grpc;

namespace Persistify.Protos.Templates;

[ServiceContract]
public interface ITemplateService
{
    [OperationContract]
    ValueTask<AddTemplateResponse> Add(AddTemplateRequest request, CallContext context);

    [OperationContract]
    ValueTask<ListTemplatesResponse> List(ListTemplatesRequest request, CallContext context);

    [OperationContract]
    ValueTask<DeleteTemplateResponse> Delete(DeleteTemplateRequest request, CallContext context);
}
