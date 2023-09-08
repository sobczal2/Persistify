using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Internal.Management;
using Persistify.Server.HostedServices.Abstractions;

namespace Persistify.Server.HostedServices.Actions;

public class ManagementStartupAction : IStartupAction
{
    private readonly InitializeDocumentManagersCommand _initializeDocumentManagersCommand;
    private readonly InitializeTemplateManagerCommand _initializeTemplateManagerCommand;

    public ManagementStartupAction(
        InitializeTemplateManagerCommand initializeTemplateManagerCommand,
        InitializeDocumentManagersCommand initializeDocumentManagersCommand
    )
    {
        _initializeTemplateManagerCommand = initializeTemplateManagerCommand;
        _initializeDocumentManagersCommand = initializeDocumentManagersCommand;
    }

    public string Name => "ManagementStartupAction";

    public async ValueTask PerformStartupActionAsync(CancellationToken cancellationToken)
    {
        await _initializeTemplateManagerCommand.RunInTransactionAsync(new EmptyRequest(), cancellationToken);
        await _initializeDocumentManagersCommand.RunInTransactionAsync(new EmptyRequest(), cancellationToken);
    }
}
