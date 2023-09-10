using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Internal.Management;

namespace Persistify.Server.HostedServices;

public class StartupActionHostedService : BackgroundService
{
    private readonly ILogger<StartupActionHostedService> _logger;
    private readonly SetupFileSystemCommand _setupFileSystemCommand;
    private readonly InitializeTemplateManagerCommand _initializeTemplateManagerCommand;
    private readonly InitializeDocumentManagersCommand _initializeDocumentManagersCommand;
    private readonly InitializeUserManagerCommand _initializeUserManagerCommand;

    public StartupActionHostedService(
        ILogger<StartupActionHostedService> logger,
        SetupFileSystemCommand setupFileSystemCommand,
        InitializeTemplateManagerCommand initializeTemplateManagerCommand,
        InitializeDocumentManagersCommand initializeDocumentManagersCommand,
        InitializeUserManagerCommand initializeUserManagerCommand
    )
    {
        _logger = logger;
        _setupFileSystemCommand = setupFileSystemCommand;
        _initializeTemplateManagerCommand = initializeTemplateManagerCommand;
        _initializeDocumentManagersCommand = initializeDocumentManagersCommand;
        _initializeUserManagerCommand = initializeUserManagerCommand;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Executing startup actions");
        await _setupFileSystemCommand.RunInTransactionAsync(new EmptyRequest(), stoppingToken);
        await _initializeTemplateManagerCommand.RunInTransactionAsync(new EmptyRequest(), stoppingToken);
        await _initializeDocumentManagersCommand.RunInTransactionAsync(new EmptyRequest(), stoppingToken);
        await _initializeUserManagerCommand.RunInTransactionAsync(new EmptyRequest(), stoppingToken);
        _logger.LogInformation("Startup actions executed");
    }
}
