using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.Commands.Users;

public class ChangeUserPasswordCommand : Command<ChangeUserPasswordRequest, ChangeUserPasswordResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IUserManager _userManager;

    public ChangeUserPasswordCommand(
        ICommandContext<ChangeUserPasswordRequest> commandContext,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override async ValueTask RunAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .GetAsync(request.Username) ?? throw new PersistifyInternalException();

        var (hash, salt) = _passwordService.HashPassword(request.Password);

        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _userManager.Update(user);
    }

    protected override ChangeUserPasswordResponse GetResponse()
    {
        return new ChangeUserPasswordResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ChangeUserPasswordRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(ChangeUserPasswordRequest request)
    {
        return Permission.UserWrite;
    }
}
