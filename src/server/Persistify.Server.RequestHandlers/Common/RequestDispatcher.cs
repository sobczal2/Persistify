using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Requests.Common;
using Persistify.Responses.Common;

namespace Persistify.Server.CommandHandlers.Common;

public class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> DispatchAsync<TRequest, TResponse>(
        TRequest request,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest<TResponse>
        where TResponse : IResponse
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        return handler.HandleAsync(request, claimsPrincipal, cancellationToken);
    }
}
