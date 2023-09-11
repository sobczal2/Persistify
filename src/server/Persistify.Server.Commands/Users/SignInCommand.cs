using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Commands.Users;

public class SignInCommand : Command<SignInRequest, SignInResponse>
{
    private readonly IUserManager _userManager;
    private readonly IPasswordService _passwordService;
    private SignInResponse? _response;

    public SignInCommand(
        IValidator<SignInRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override async ValueTask RunAsync(SignInRequest data, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(data.Username);

        if (user is null)
        {
            throw new ValidationException(nameof(SignInRequest.Username), SharedErrorMessages.InvalidCredentials);
        }

        var passwordCorrect = _passwordService.VerifyPassword(data.Password, user.PasswordHash, user.PasswordSalt);

        if (!passwordCorrect)
        {
            throw new ValidationException(nameof(SignInRequest.Username), SharedErrorMessages.InvalidCredentials);
        }

        var (accessToken, refreshToken) = await _userManager.CreateTokens(user.Id);

        _response = new SignInResponse
        {
            Username = user.Username, Role = user.Role, AccessToken = accessToken, RefreshToken = refreshToken
        };
    }

    protected override SignInResponse GetResponse()
    {
        return _response ?? throw new PersistifyInternalException();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(SignInRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }
}
