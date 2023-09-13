using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Users;

public class GetUserCommand : Command<GetUserRequest, GetUserResponse>
{
    private readonly IUserManager _userManager;
    private User? _user;

    public GetUserCommand(
        IValidator<GetUserRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        IUserManager userManager
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(GetUserRequest data, CancellationToken cancellationToken)
    {
        _user = await _userManager.GetAsync(data.Username);
        if (_user is null)
        {
            throw new ValidationException(nameof(GetUserRequest.Username), $"User {data.Username} not found");
        }
    }

    protected override GetUserResponse GetResponse()
    {
        if (_user is null)
        {
            throw new PersistifyInternalException();
        }

        return new GetUserResponse
        {
            Username = _user.Username,
            Permission = (int)_user.Permission
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetUserRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _userManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetUserRequest data)
    {
        return Permission.UserRead;
    }
}
