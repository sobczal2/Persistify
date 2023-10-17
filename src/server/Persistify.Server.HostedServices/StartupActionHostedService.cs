using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Security;

namespace Persistify.Server.HostedServices;

public class StartupActionHostedService : BackgroundService
{
    private readonly ILogger<StartupActionHostedService> _logger;
    private readonly IRequestDispatcher _requestDispatcher;

    public StartupActionHostedService(
        ILogger<StartupActionHostedService> logger,
        IRequestDispatcher requestDispatcher
    )
    {
        _logger = logger;
        _requestDispatcher = requestDispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Executing startup actions");
        var internalClaimsPrincipal = ClaimsPrincipalExtensions.InternalClaimsPrincipal;
        await _requestDispatcher.DispatchAsync<SetupFileSystemRequest, SetupFileSystemResponse>(new SetupFileSystemRequest(),
            internalClaimsPrincipal, stoppingToken);
        await _requestDispatcher.DispatchAsync<InitializeTemplateManagerRequest, InitializeTemplateManagerResponse>(
            new InitializeTemplateManagerRequest(), internalClaimsPrincipal, stoppingToken);
        await _requestDispatcher.DispatchAsync<InitializeDocumentManagersRequest, InitializeDocumentManagersResponse>(
            new InitializeDocumentManagersRequest(), internalClaimsPrincipal, stoppingToken);
        await _requestDispatcher.DispatchAsync<InitializeUserManagerRequest, InitializeUserManagerResponse>(
            new InitializeUserManagerRequest(), internalClaimsPrincipal, stoppingToken);
        await _requestDispatcher.DispatchAsync<EnsureRootUserExistsRequest, EnsureRootUserExistsResponse>(
            new EnsureRootUserExistsRequest(), internalClaimsPrincipal, stoppingToken);
        _logger.LogInformation("Startup actions executed");
    }
}
