using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Dtos.Mappers;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Users;

public class GetUserRequestHandler : RequestHandler<GetUserRequest, GetUserResponse>
{
    private readonly IUserManager _userManager;
    private User? _user;

    public GetUserRequestHandler(
        IRequestHandlerContext<GetUserRequest, GetUserResponse> requestHandlerContext,
        IUserManager userManager
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        _user = await _userManager.GetAsync(request.Username) ??
                throw new NotFoundPersistifyException(nameof(GetUserRequest), UserErrorMessages.UserNotFound);
    }

    protected override GetUserResponse GetResponse()
    {
        if (_user is null)
        {
            throw new InternalPersistifyException(nameof(GetUserRequest));
        }

        return new GetUserResponse { User = UserMapper.Map(_user) };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetUserRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _userManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetUserRequest request)
    {
        return Permission.UserRead;
    }
}
