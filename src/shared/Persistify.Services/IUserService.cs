using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<GetUserResponse> GetUserAsync(GetUserRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<SignInResponse> SignInAsync(SignInRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<SetPermissionResponse> SetPermissionAsync(SetPermissionRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<ChangeUserPasswordResponse> ChangeUserPasswordAsync(ChangeUserPasswordRequest request,
        CallContext callContext);
}
