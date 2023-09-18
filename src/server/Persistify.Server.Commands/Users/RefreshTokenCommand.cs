using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Users;

namespace Persistify.Server.Commands.Users;

public class RefreshTokenCommand : Command<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IUserManager _userManager;
    private RefreshTokenResponse? _response;

    public RefreshTokenCommand(
        ICommandContext<RefreshTokenRequest> commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username) ?? throw new PersistifyInternalException();

        if (await _userManager.CheckRefreshToken(user.Id, request.RefreshToken))
        {
            var (accessToken, refreshToken) = await _userManager.CreateTokens(user.Id);

            _response = new RefreshTokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        else
        {
            throw new ValidationException(nameof(RefreshTokenRequest.RefreshToken),
                UserErrorMessages.InvalidRefreshToken);
        }
    }

    protected override RefreshTokenResponse GetResponse()
    {
        return _response ?? throw new PersistifyInternalException();
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
