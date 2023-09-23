﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;
using Persistify.Server.Validation.Users;

namespace Persistify.Server.Commands.Users;

public class SignInCommand : Command<SignInRequest, SignInResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IUserManager _userManager;
    private SignInResponse? _response;

    public SignInCommand(
        ICommandContext<SignInRequest> commandContext,
        IUserManager userManager,
        IPasswordService passwordService
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    protected override async ValueTask RunAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username) ?? throw new PersistifyInternalException();

        var passwordCorrect = _passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!passwordCorrect)
        {
            throw ValidationException(nameof(SignInRequest.Username), UserErrorMessages.InvalidCredentials);
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
        return _response ?? throw new PersistifyInternalException();
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