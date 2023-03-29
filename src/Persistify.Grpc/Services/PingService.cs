using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Protos;

namespace Persistify.Grpc.Services;

public class PingService : Protos.PingService.PingServiceBase
{
    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse { Message = "pong" });
    }

    public override Task<ValidationErrorPingResponse> ValidationErrorPing(
        ValidationErrorPingRequest request,
        ServerCallContext context
    )
    {
        return base.ValidationErrorPing(request, context);
    }
}