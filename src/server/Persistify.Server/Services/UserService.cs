using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Users;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class UserService : IUserService
{
    private readonly CreateUserCommand _createUserCommand;
    private readonly GetUserCommand _getUserCommand;

    public UserService(
        CreateUserCommand createUserCommand,
        GetUserCommand getUserCommand
        )
    {
        _createUserCommand = createUserCommand;
        _getUserCommand = getUserCommand;
    }
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext context)
    {
        return await _createUserCommand.RunInTransactionAsync(request, context.CancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<GetUserResponse> GetUserAsync(GetUserRequest request, CallContext context)
    {
        return await _getUserCommand.RunInTransactionAsync(request, context.CancellationToken).ConfigureAwait(false);
    }
}
