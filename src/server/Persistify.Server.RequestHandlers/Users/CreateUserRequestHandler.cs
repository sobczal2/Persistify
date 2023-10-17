using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.CommandHandlers.Users;

public class CreateUserRequestHandler : RequestHandler<CreateUserRequest, CreateUserResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IUserManager _userManager;

    public CreateUserRequestHandler(
        IRequestHandlerContext<CreateUserRequest, CreateUserResponse> requestHandlerContext,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override ValueTask RunAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
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
