using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Protos.Templates;

namespace Persistify.Server.Services;

public class TemplateService : Protos.Templates.TemplateService.TemplateServiceBase
{
    public override Task<AddTemplateResponse> Add(AddTemplateRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Aborted, "Hello World!"));
    }
}
