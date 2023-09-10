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

    public UserService(
        CreateUserCommand createUserCommand
        )
    {
        _createUserCommand = createUserCommand;
    }
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext context)
    {
        return await _createUserCommand.RunInTransactionAsync(request, context.CancellationToken).ConfigureAwait(false);
    }
}
