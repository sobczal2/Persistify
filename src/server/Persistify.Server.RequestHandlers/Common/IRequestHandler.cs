using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Common;
using Persistify.Responses.Common;

namespace Persistify.Server.CommandHandlers.Common;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{
    ValueTask<TResponse> HandleAsync(
        TRequest request,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    );
}
