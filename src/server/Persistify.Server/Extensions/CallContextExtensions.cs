using System.Security.Claims;
using Grpc.Core;
using Persistify.Server.Security;
using ProtoBuf.Grpc;

namespace Persistify.Server.Extensions;

public static class CallContextExtensions
{
    public static ClaimsPrincipal GetClaimsPrincipal(this CallContext callContext)
    {
        return callContext.ServerCallContext?.GetHttpContext().User
               ?? ClaimsPrincipalExtensions.UnknownClaimsPrincipal;
    }
}
