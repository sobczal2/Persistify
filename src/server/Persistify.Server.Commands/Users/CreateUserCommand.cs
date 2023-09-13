using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Users;

public class CreateUserCommand : Command<CreateUserRequest, CreateUserResponse>
{
    private readonly IUserManager _userManager;
    private readonly IPasswordService _passwordService;

    public CreateUserCommand(
        IValidator<CreateUserRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override ValueTask RunAsync(CreateUserRequest data, CancellationToken cancellationToken)
    {
        if (_userManager.Exists(data.Username))
        {
            throw new ValidationException(nameof(CreateUserRequest.Username), $"User {data.Username} already exists");
        }

        var (hash, salt) = _passwordService.HashPassword(data.Password);

        var user = new User { Username = data.Username, PasswordHash = hash, PasswordSalt = salt, Permission = Permission.None };

        _userManager.Add(user);

        return ValueTask.CompletedTask;
    }

    protected override CreateUserResponse GetResponse()
    {
        return new CreateUserResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(CreateUserRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(CreateUserRequest data)
    {
        return Permission.UserWrite;
    }
}
