using Persistify.Client.Core;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using ProtoBuf.Grpc;

namespace Persistify.Client.Users;

public static class UsersClientExtensions
{
    public static async Task<SignInResponse> SignInAsync(this IPersistifyClient persistifyClient,
        SignInRequest signInRequest,
        CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.SignInAsync(userService, signInRequest, callContext);
    }

    public static async Task<RefreshTokenResponse> RefreshTokenAsync(this IPersistifyClient persistifyClient,
        RefreshTokenRequest refreshTokenRequest, CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.RefreshTokenAsync(userService, refreshTokenRequest, callContext);
    }

    public static async Task<CreateUserResponse> CreateUserAsync(this IPersistifyClient persistifyClient,
        CreateUserRequest createUserRequest, CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.CreateUserAsync(userService, createUserRequest, callContext);
    }

    public static async Task<GetUserResponse> GetUserAsync(this IPersistifyClient persistifyClient,
        GetUserRequest getUserRequest,
        CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.GetUserAsync(userService, getUserRequest, callContext);
    }

    public static async Task<SetPermissionResponse> SetPermissionAsync(this IPersistifyClient persistifyClient,
        SetPermissionRequest setPermissionRequest, CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.SetPermissionAsync(userService, setPermissionRequest, callContext);
    }

    public static async Task<DeleteUserResponse> DeleteUserAsync(this IPersistifyClient persistifyClient,
        DeleteUserRequest deleteUserRequest, CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.DeleteUserAsync(userService, deleteUserRequest, callContext);
    }

    public static async Task<ChangeUserPasswordResponse> ChangeUserPasswordAsync(
        this IPersistifyClient persistifyClient,
        ChangeUserPasswordRequest changeUserPasswordRequest, CallContext? callContext = default)
    {
        var userService = persistifyClient.Users.GetService();
        return await persistifyClient.Users.ChangeUserPasswordAsync(userService, changeUserPasswordRequest,
            callContext);
    }
}
