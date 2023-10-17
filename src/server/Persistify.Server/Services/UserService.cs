using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.CommandHandlers.Users;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class UserService : IUserService
{
    private readonly IRequestDispatcher _requestDispatcher;

    public UserService(
        IRequestDispatcher requestDispatcher
    )
    {
        _requestDispatcher = requestDispatcher;
    }

    [Authorize]
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<CreateUserRequest, CreateUserResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetUserResponse> GetUserAsync(GetUserRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<GetUserRequest, GetUserResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    public async ValueTask<SignInResponse> SignInAsync(SignInRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<SignInRequest, SignInResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<SetPermissionResponse> SetPermissionAsync(SetPermissionRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<SetPermissionRequest, SetPermissionResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<DeleteUserRequest, DeleteUserResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    public async ValueTask<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<RefreshTokenRequest, RefreshTokenResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ChangeUserPasswordResponse> ChangeUserPasswordAsync(ChangeUserPasswordRequest request,
        CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<ChangeUserPasswordRequest, ChangeUserPasswordResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ListUsersResponse> ListUsersAsync(ListUsersRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<ListUsersRequest, ListUsersResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ExistsUserResponse> ExistsUserAsync(ExistsUserRequest request, CallContext callContext)
    {
        return await _requestDispatcher.DispatchAsync<ExistsUserRequest, ExistsUserResponse>(request,
            callContext.GetClaimsPrincipal(), callContext.CancellationToken);
    }
}
