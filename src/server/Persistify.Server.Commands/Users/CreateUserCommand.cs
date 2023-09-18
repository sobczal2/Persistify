using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Users;

namespace Persistify.Server.Commands.Users;

public class CreateUserCommand : Command<CreateUserRequest, CreateUserResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IUserManager _userManager;

    public CreateUserCommand(
        ICommandContext<CreateUserRequest> commandContext,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override ValueTask RunAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (_userManager.Exists(request.Username))
        {
            throw new ValidationException(nameof(CreateUserRequest.Username), UserErrorMessages.UserAlreadyExists);
        }

        var (hash, salt) = _passwordService.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username, PasswordHash = hash, PasswordSalt = salt, Permission = Permission.None
        };

        _userManager.Add(user);

        return ValueTask.CompletedTask;
    }

    protected override CreateUserResponse GetResponse()
    {
        return new CreateUserResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(CreateUserRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(CreateUserRequest request)
    {
        return Permission.UserWrite;
    }
}
