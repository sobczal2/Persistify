using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.Util;
using Persistify.Responses.Util;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface IUtilService
{
    [OperationContract]
    ValueTask<RunGcResponse> RunGcAsync(
        RunGcRequest request,
        CallContext callContext
    );
}
