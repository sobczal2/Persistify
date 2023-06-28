using Grpc.Core;
using Persistify.Protos.Templates;

namespace Persistify.Server.Services;

public class TemplateService : Persistify.Protos.Templates.TemplateService.TemplateServiceBase
{
    public override Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
    {
        return base.Create(request, context);
    }
}