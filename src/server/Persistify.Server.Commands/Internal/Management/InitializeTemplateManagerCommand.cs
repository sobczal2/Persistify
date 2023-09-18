using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class InitializeTemplateManagerCommand : Command
{
    private readonly ITemplateManager _templateManager;

    public InitializeTemplateManagerCommand(
        ICommandContext commandContext,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        _templateManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(EmptyRequest request)
    {
        return Permission.TemplateWrite;
    }
}
