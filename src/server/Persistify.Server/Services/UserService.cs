using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Users;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class UserService : IUserService
{
    private readonly CreateUserCommand _createUserCommand;
    private readonly GetUserCommand _getUserCommand;
    private readonly SetPermissionCommand _setPermissionCommand;
    private readonly SignInCommand _signInCommand;

    public UserService(
        CreateUserCommand createUserCommand,
        GetUserCommand getUserCommand,
        SignInCommand signInCommand,
        SetPermissionCommand setPermissionCommand
    )
    {
        _createUserCommand = createUserCommand;
        _getUserCommand = getUserCommand;
        _signInCommand = signInCommand;
        _setPermissionCommand = setPermissionCommand;
    }

    [Authorize]
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext callContext)
    {
        return await _createUserCommand
            .RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken)
            .ConfigureAwait(false);
    }

    [Authorize]
    public async ValueTask<GetUserResponse> GetUserAsync(GetUserRequest request, CallContext callContext)
    {
        return await _getUserCommand
            .RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<SignInResponse> SignInAsync(SignInRequest request, CallContext callContext)
    {
        return await _signInCommand
            .RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken)
            .ConfigureAwait(false);
    }

    [Authorize]
    public async ValueTask<SetPermissionResponse> SetPermissionAsync(SetPermissionRequest request,
        CallContext callContext)
    {
        return await _setPermissionCommand
            .RunInTransactionAsync(request, callContext.GetClaimsPrincipal(), callContext.CancellationToken)
            .ConfigureAwait(false);
    }
}
