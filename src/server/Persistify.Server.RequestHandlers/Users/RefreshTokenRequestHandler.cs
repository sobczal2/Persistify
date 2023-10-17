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
using Persistify.Server.Validation.Users;

namespace Persistify.Server.CommandHandlers.Users;

public class RefreshTokenRequestHandler : RequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IUserManager _userManager;
    private RefreshTokenResponse? _response;

    public RefreshTokenRequestHandler(
        IRequestHandlerContext<RefreshTokenRequest, RefreshTokenResponse> requestHandlerContext,
        IUserManager userManager
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username) ??
                   throw new NotFoundPersistifyException(nameof(RefreshTokenRequest), UserErrorMessages.UserNotFound);

        if (await _userManager.CheckRefreshToken(user.Id, request.RefreshToken))
        {
            var (accessToken, refreshToken) = await _userManager.CreateTokens(user.Id);

            _response = new RefreshTokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        else
        {
            throw new DynamicValidationPersistifyException(nameof(RefreshTokenRequest.RefreshToken),
                UserErrorMessages.InvalidRefreshToken);
        }
    }

    protected override RefreshTokenResponse GetResponse()
    {
        return _response ?? throw new InternalPersistifyException(nameof(RefreshTokenRequest));
    }

    protected override TransactionDescriptor GetTransactionDescriptor(RefreshTokenRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(RefreshTokenRequest request)
    {
        return Permission.None;
    }
}
