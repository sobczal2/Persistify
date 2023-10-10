using Persistify.Client.Core;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Users;

public class UsersClient : SubClient<IUserService>, IUsersClient
{
    internal UsersClient(PersistifyClient persistifyClient) : base(persistifyClient)
    {
    }

    public async Task<SignInResponse> SignInAsync(IUserService userService, SignInRequest signInRequest, CallContext? callContext = default)
    {
        return await userService.SignInAsync(
            signInRequest,
            callContext ?? new CallContext()
        );
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(IUserService userService,
        RefreshTokenRequest refreshTokenRequest,
        CallContext? callContext = default)
    {
        return await userService.RefreshTokenAsync(
            refreshTokenRequest,
            callContext ?? new CallContext()
        );
    }

    public async Task<CreateUserResponse> CreateUserAsync(IUserService userService, CreateUserRequest createUserRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<CreateUserResponse>(
            async cc => await userService.CreateUserAsync(createUserRequest, cc), callContext
        );
    }

    public async Task<GetUserResponse> GetUserAsync(IUserService userService, GetUserRequest getUserRequest,
        CallContext? callContext = default)
    {
        await userService.GetUserAsync(getUserRequest, callContext ?? new CallContext());
        return await PersistifyClient.CallAuthenticatedServiceAsync<GetUserResponse>(
            async cc => await userService.GetUserAsync(getUserRequest, cc), callContext
        );
    }

    public async Task<SetPermissionResponse> SetPermissionAsync(IUserService userService, SetPermissionRequest setPermissionRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<SetPermissionResponse>(
            async cc => await userService.SetPermissionAsync(setPermissionRequest, cc), callContext
        );
    }

    public async Task<DeleteUserResponse> DeleteUserAsync(IUserService userService, DeleteUserRequest deleteUserRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<DeleteUserResponse>(
            async cc => await userService.DeleteUserAsync(deleteUserRequest, cc), callContext
        );
    }

    public async Task<ChangeUserPasswordResponse> ChangeUserPasswordAsync(IUserService userService, ChangeUserPasswordRequest changeUserPasswordRequest,
        CallContext? callContext = default)
    {
        return await PersistifyClient.CallAuthenticatedServiceAsync<ChangeUserPasswordResponse>(
            async cc => await userService.ChangeUserPasswordAsync(changeUserPasswordRequest, cc), callContext
        );
    }
}
