using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Users;

public class UsersClient : SubClient<IUserService>, IUsersClient
{
    internal UsersClient(PersistifyLowLevelClient persistifyLowLevelClient)
        : base(persistifyLowLevelClient) { }

    public async Task<Result<SignInResponse>> SignInAsync(
        IUserService userService,
        SignInRequest signInRequest,
        CallContext? callContext = default
    )
    {
        return await Result<SignInResponse>.FromAsync(
            async () =>
                await userService.SignInAsync(signInRequest, callContext ?? new CallContext())
        );
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(
        IUserService userService,
        RefreshTokenRequest refreshTokenRequest,
        CallContext? callContext = default
    )
    {
        return await Result<RefreshTokenResponse>.FromAsync(
            async () =>
                await userService.RefreshTokenAsync(
                    refreshTokenRequest,
                    callContext ?? new CallContext()
                )
        );
    }

    public async Task<Result<CreateUserResponse>> CreateUserAsync(
        IUserService userService,
        CreateUserRequest createUserRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<CreateUserResponse>(
            async cc =>
                await Result<CreateUserResponse>.FromAsync(
                    async () => await userService.CreateUserAsync(createUserRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<GetUserResponse>> GetUserAsync(
        IUserService userService,
        GetUserRequest getUserRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<GetUserResponse>(
            async cc =>
                await Result<GetUserResponse>.FromAsync(
                    async () => await userService.GetUserAsync(getUserRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<SetPermissionResponse>> SetPermissionAsync(
        IUserService userService,
        SetPermissionRequest setPermissionRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<SetPermissionResponse>(
            async cc =>
                await Result<SetPermissionResponse>.FromAsync(
                    async () => await userService.SetPermissionAsync(setPermissionRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<DeleteUserResponse>> DeleteUserAsync(
        IUserService userService,
        DeleteUserRequest deleteUserRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<DeleteUserResponse>(
            async cc =>
                await Result<DeleteUserResponse>.FromAsync(
                    async () => await userService.DeleteUserAsync(deleteUserRequest, cc)
                ),
            callContext
        );
    }

    public async Task<Result<ChangeUserPasswordResponse>> ChangeUserPasswordAsync(
        IUserService userService,
        ChangeUserPasswordRequest changeUserPasswordRequest,
        CallContext? callContext = default
    )
    {
        return await PersistifyLowLevelClient.CallAuthenticatedServiceAsync<ChangeUserPasswordResponse>(
            async cc =>
                await Result<ChangeUserPasswordResponse>.FromAsync(
                    async () =>
                        await userService.ChangeUserPasswordAsync(changeUserPasswordRequest, cc)
                ),
            callContext
        );
    }
}
