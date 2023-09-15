using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.Commands.Internal.Management;

public class EnsureRootUserExistsCommand : Command
{
    private readonly IPasswordService _passwordService;
    private readonly IOptions<RootSettings> _rootSettingsOptions;
    private readonly IUserManager _userManager;

    public EnsureRootUserExistsCommand(
        ICommandContext commandContext,
        IUserManager userManager,
        IPasswordService passwordService,
        IOptions<RootSettings> rootSettingsOptions
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
        _rootSettingsOptions = rootSettingsOptions;
    }

    protected override ValueTask RunAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        if (!_userManager.Exists(_rootSettingsOptions.Value.Username))
        {
            var (hash, salt) = _passwordService.HashPassword(_rootSettingsOptions.Value.Password);

            var user = new User
            {
                Username = _rootSettingsOptions.Value.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Permission = Permission.All
            };

            _userManager.Add(user);
        }

        return ValueTask.CompletedTask;
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(EmptyRequest request)
    {
        return Permission.UserWrite;
    }
}
