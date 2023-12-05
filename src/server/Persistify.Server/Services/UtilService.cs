using System;
using System.Threading.Tasks;
using Persistify.Requests.Util;
using Persistify.Responses.Util;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class UtilService : IUtilService
{
    public ValueTask<RunGcResponse> RunGcAsync(RunGcRequest request, CallContext callContext)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return new ValueTask<RunGcResponse>(new RunGcResponse());
    }
}
