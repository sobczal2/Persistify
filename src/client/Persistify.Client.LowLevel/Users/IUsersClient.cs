using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Users;

public interface IUsersClient
{
    IUserService GetService();

    Task<Result<SignInResponse>> SignInAsync(
        IUserService userService,
        SignInRequest signInRequest,
        CallContext? callContext = default
    );

    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(
        IUserService userService,
        RefreshTokenRequest refreshTokenRequest,
        CallContext? callContext = default
    );

    Task<Result<CreateUserResponse>> CreateUserAsync(
        IUserService userService,
        CreateUserRequest createUserRequest,
        CallContext? callContext = default
    );

    Task<Result<GetUserResponse>> GetUserAsync(
        IUserService userService,
        GetUserRequest getUserRequest,
        CallContext? callContext = default
    );

    Task<Result<SetPermissionResponse>> SetPermissionAsync(
        IUserService userService,
        SetPermissionRequest setPermissionRequest,
        CallContext? callContext = default
    );

    Task<Result<DeleteUserResponse>> DeleteUserAsync(
        IUserService userService,
        DeleteUserRequest deleteUserRequest,
        CallContext? callContext = default
    );

    Task<Result<ChangeUserPasswordResponse>> ChangeUserPasswordAsync(
        IUserService userService,
        ChangeUserPasswordRequest changeUserPasswordRequest,
        CallContext? callContext = default
    );
}
