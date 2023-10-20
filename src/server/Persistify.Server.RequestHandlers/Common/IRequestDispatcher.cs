using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Common;
using Persistify.Responses.Common;

namespace Persistify.Server.CommandHandlers.Common;

public interface IRequestDispatcher
{
    ValueTask<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
        where TResponse : IResponse;
}
