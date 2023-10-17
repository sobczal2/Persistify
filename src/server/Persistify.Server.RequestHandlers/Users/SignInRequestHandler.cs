using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;
using Persistify.Server.Validation.Users;

namespace Persistify.Server.CommandHandlers.Users;

public class SignInRequestHandler : RequestHandler<SignInRequest, SignInResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IUserManager _userManager;
    private SignInResponse? _response;

    public SignInRequestHandler(
        IRequestHandlerContext<SignInRequest, SignInResponse> requestHandlerContext,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override async ValueTask RunAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username);

        if (user is null)
        {
            throw new UnauthenticatedPersistifyException(nameof(SignInRequest.Username),
                UserErrorMessages.InvalidCredentials);
        }

        var passwordCorrect = _passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!passwordCorrect)
        {
            throw new UnauthenticatedPersistifyException(nameof(SignInRequest.Username),
                UserErrorMessages.InvalidCredentials);
        }

        var (accessToken, refreshToken) = await _userManager.CreateTokens(user.Id);

        _response = new SignInResponse
        {
            Username = user.Username,
            Permission = (int)user.Permission,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    protected override SignInResponse GetResponse()
    {
        return _response ?? throw new InternalPersistifyException(nameof(SignInRequest));
    }

    protected override TransactionDescriptor GetTransactionDescriptor(SignInRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(SignInRequest request)
    {
        return Permission.None;
    }
}
