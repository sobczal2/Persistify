using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Persistify.Protos;
using Persistify.Stores.User;

namespace Persistify.Server.Services;

public class AuthService : Protos.AuthService.AuthServiceBase
{
    private readonly IUserStore _userStore;

    public AuthService(
        IUserStore userStore)
    {
        _userStore = userStore;
    }

    public override Task<SignInResponseProto> Login(SignInRequestProto request, ServerCallContext context)
    {
        if (_userStore.Verify(request.Username, request.Password))
            return Task.FromResult(new SignInResponseProto
            {
                Token = new TokenResponseProto
                {
                    AccessToken = _userStore.GenerateJwtToken(request.Username),
                    RefreshToken = _userStore.GenerateRefreshToken(request.Username)
                }
            });

        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid username or password"));
    }

    [Authorize(Roles = UserRoles.SuperUser)]
    public override Task<CreateUserResponseProto> CreateUser(CreateUserRequestProto request, ServerCallContext context)
    {
        if (request.Username.Length < 4)
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Username must be at least 4 characters long"));
        if (request.Password.Length < 8)
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Password must be at least 8 characters long"));
        if (_userStore.Exists(request.Username))
            throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists"));
        _userStore.Create(request.Username, request.Password);
        return Task.FromResult(new CreateUserResponseProto());
    }

    [Authorize(Roles = UserRoles.SuperUser)]
    public override Task<DeleteUserResponseProto> DeleteUser(DeleteUserRequestProto request, ServerCallContext context)
    {
        if (!_userStore.Exists(request.Username))
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        if (_userStore.IsSuperUser(request.Username))
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Cannot delete super user"));
        _userStore.Delete(request.Username);
        return Task.FromResult(new DeleteUserResponseProto());
    }

    public override Task<RefreshTokenResponseProto> RefreshToken(RefreshTokenRequestProto request,
        ServerCallContext context)
    {
        if (_userStore.VerifyRefreshToken(request.Username, request.RefreshToken))
            return Task.FromResult(new RefreshTokenResponseProto
            {
                Token = new TokenResponseProto
                {
                    AccessToken = _userStore.GenerateJwtToken(request.Username),
                    RefreshToken = _userStore.GenerateRefreshToken(request.Username)
                }
            });

        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid refresh token"));
    }
}