using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Client.Users;

public interface IUsersClient
{
    IUserService GetService();
    Task<SignInResponse> SignInAsync(IUserService userService, SignInRequest signInRequest, CallContext? callContext = default);
    Task<RefreshTokenResponse> RefreshTokenAsync(IUserService userService, RefreshTokenRequest refreshTokenRequest, CallContext? callContext = default);
    Task<CreateUserResponse> CreateUserAsync(IUserService userService, CreateUserRequest createUserRequest, CallContext? callContext = default);
    Task<GetUserResponse> GetUserAsync(IUserService userService, GetUserRequest getUserRequest, CallContext? callContext = default);
    Task<SetPermissionResponse> SetPermissionAsync(IUserService userService, SetPermissionRequest setPermissionRequest, CallContext? callContext = default);
    Task<DeleteUserResponse> DeleteUserAsync(IUserService userService, DeleteUserRequest deleteUserRequest, CallContext? callContext = default);
    Task<ChangeUserPasswordResponse> ChangeUserPasswordAsync(IUserService userService, ChangeUserPasswordRequest changeUserPasswordRequest, CallContext? callContext = default);
}
