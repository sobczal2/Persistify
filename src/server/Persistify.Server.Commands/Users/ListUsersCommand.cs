using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Dtos.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Users;

public class ListUsersCommand : Command<ListUsersRequest, ListUsersResponse>
{
    private readonly IUserManager _userManager;
    private List<UserDto>? _userDtos;
    private int? _totalCount;

    public ListUsersCommand(
        ICommandContext<ListUsersRequest> commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(ListUsersRequest request, CancellationToken cancellationToken)
    {
        var skip = request.Pagination.PageNumber * request.Pagination.PageSize;
        var take = request.Pagination.PageSize;

        _userDtos = await _userManager
            .ListAsync(take, skip)
            .Select(x => new UserDto { Username = x.Username, Permission = (int)x.Permission })
            .ToListAsync(cancellationToken);

        _totalCount = _userManager.Count();
    }

    protected override ListUsersResponse GetResponse()
    {
        return new ListUsersResponse
        {
            Users = _userDtos ?? throw new InternalPersistifyException(nameof(ListUsersRequest)),
            TotalCount = _totalCount ?? throw new InternalPersistifyException(nameof(ListUsersRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ListUsersRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _userManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ListUsersRequest request)
    {
        return Permission.UserRead;
    }
}
