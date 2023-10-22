using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Domain.Users;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.CommandHandlers.Internal;

public class
    EnsureRootUserExistsRequestHandler : RequestHandler<EnsureRootUserExistsRequest, EnsureRootUserExistsResponse>
{
    private readonly IPasswordService _passwordService;
    private readonly IOptions<RootSettings> _rootSettingsOptions;
    private readonly IUserManager _userManager;

    public EnsureRootUserExistsRequestHandler(
        IRequestHandlerContext<EnsureRootUserExistsRequest, EnsureRootUserExistsResponse> requestHandlerContext,
        IUserManager userManager,
        IPasswordService passwordService,
        IOptions<RootSettings> rootSettingsOptions
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
        _passwordService = passwordService;
        _rootSettingsOptions = rootSettingsOptions;
    }

    protected override ValueTask RunAsync(EnsureRootUserExistsRequest request, CancellationToken cancellationToken)
    {
        if (!_userManager.Exists(_rootSettingsOptions.Value.Username))
        {
            var (hash, salt) = _passwordService.HashPassword(_rootSettingsOptions.Value.Password);

            var user = new User
            {
                Username = _rootSettingsOptions.Value.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Permission = Permission.All
            };

            _userManager.Add(user);
        }

        return ValueTask.CompletedTask;
    }

    protected override EnsureRootUserExistsResponse GetResponse()
    {
        return new EnsureRootUserExistsResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EnsureRootUserExistsRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(EnsureRootUserExistsRequest request)
    {
        return Permission.UserWrite;
    }
}
