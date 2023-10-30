using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Users;

public static class UsersClientExtensions
{
    public static async Task<Result<SignInResponse>> SignInAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        SignInRequest signInRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.SignInAsync(
            userService,
            signInRequest,
            callContext
        );
    }

    public static async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        RefreshTokenRequest refreshTokenRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.RefreshTokenAsync(
            userService,
            refreshTokenRequest,
            callContext
        );
    }

    public static async Task<Result<CreateUserResponse>> CreateUserAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        CreateUserRequest createUserRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.CreateUserAsync(
            userService,
            createUserRequest,
            callContext
        );
    }

    public static async Task<Result<GetUserResponse>> GetUserAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        GetUserRequest getUserRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.GetUserAsync(
            userService,
            getUserRequest,
            callContext
        );
    }

    public static async Task<Result<SetPermissionResponse>> SetPermissionAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        SetPermissionRequest setPermissionRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.SetPermissionAsync(
            userService,
            setPermissionRequest,
            callContext
        );
    }

    public static async Task<Result<DeleteUserResponse>> DeleteUserAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        DeleteUserRequest deleteUserRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.DeleteUserAsync(
            userService,
            deleteUserRequest,
            callContext
        );
    }

    public static async Task<Result<ChangeUserPasswordResponse>> ChangeUserPasswordAsync(
        this IPersistifyLowLevelClient persistifyLowLevelClient,
        ChangeUserPasswordRequest changeUserPasswordRequest,
        CallContext? callContext = default
    )
    {
        var userService = persistifyLowLevelClient.Users.GetService();
        return await persistifyLowLevelClient.Users.ChangeUserPasswordAsync(
            userService,
            changeUserPasswordRequest,
            callContext
        );
    }
}
