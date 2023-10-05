using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Users;
using Persistify.Helpers.Time;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Files;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Security;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Users;

public class UserManager : Manager, IUserManager
{
    private readonly IClock _clock;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<RefreshToken> _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly TokenSettings _tokenSettings;
    private readonly ConcurrentDictionary<string, int> _usernameIdDictionary;
    private readonly ObjectStreamRepository<User> _userRepository;
    private volatile int _count;

    public UserManager(
        ITransactionState transactionState,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IClock clock,
        ITokenService tokenService,
        IOptions<TokenSettings> tokenSettingsOptions
    ) : base(
        transactionState
    )
    {
        _clock = clock;
        _tokenService = tokenService;
        _tokenSettings = tokenSettingsOptions.Value;
        var identifierFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.IdentifierRepositoryFileName);
        var userRepositoryMainFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.UserRepositoryMainFileName);
        var userRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.UserRepositoryOffsetLengthFileName);
        var refreshTokenRepositoryMainFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.RefreshTokenRepositoryMainFileName);
        var refreshTokenRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.RefreshTokenRepositoryOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _userRepository = new ObjectStreamRepository<User>(
            userRepositoryMainFileStream,
            userRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.UserRepositorySectorSize
        );

        _refreshTokenRepository = new ObjectStreamRepository<RefreshToken>(
            refreshTokenRepositoryMainFileStream,
            refreshTokenRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.RefreshTokenRepositorySectorSize
        );

        _usernameIdDictionary = new ConcurrentDictionary<string, int>();

        _count = 0;
    }

    public override string Name => "UserManager";

    public override void Initialize()
    {
        ThrowIfCannotWrite();

        var initializeAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            if (_identifierRepository.IsValueEmpty(currentId))
            {
                await _identifierRepository.WriteAsync(0, 0, true);
            }

            _count = await _userRepository.CountAsync(true);

            var read = 0;
            const int batchSize = 1000;

            while (read < _count)
            {
                var kvList = await _userRepository.ReadRangeAsync(batchSize, read, true);

                foreach (var kv in kvList)
                {
                    _usernameIdDictionary.TryAdd(kv.Value.Username, kv.Key);
                }

                read += batchSize;
            }

            base.Initialize();
        });

        PendingActions.Enqueue(initializeAction);
    }

    public async ValueTask<User?> GetAsync(string username)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        if (!_usernameIdDictionary.TryGetValue(username, out var id))
        {
            return null;
        }

        return await _userRepository.ReadAsync(id, true);
    }

    public async ValueTask<User?> GetAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return await _userRepository.ReadAsync(id, true);
    }

    public bool Exists(string username)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _usernameIdDictionary.ContainsKey(username);
    }

    public async ValueTask<List<User>> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var kvList = await _userRepository.ReadRangeAsync(take, skip, true);
        var list = new List<User>(kvList.Count);

        foreach (var kv in kvList)
        {
            list.Add(kv.Value);
        }

        return list;
    }

    public int Count()
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _count;
    }

    public void Add(User user)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            currentId++;

            await _identifierRepository.WriteAsync(0, currentId, true);

            user.Id = currentId;

            await _userRepository.WriteAsync(currentId, user, true);

            _usernameIdDictionary.TryAdd(user.Username, currentId);

            Interlocked.Increment(ref _count);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var user = await _userRepository.ReadAsync(id, true);

        if (user == null)
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            if (await _userRepository.DeleteAsync(id, true))
            {
                _usernameIdDictionary.TryRemove(user.Username, out _);

                Interlocked.Decrement(ref _count);
            }
            else
            {
                throw new PersistifyInternalException();
            }
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }

    public async ValueTask<(string accessToken, string refreshToken)> CreateTokens(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var user = await _userRepository.ReadAsync(id, true);

        if (user is null)
        {
            throw new PersistifyInternalException();
        }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenObject = new RefreshToken
        {
            Value = refreshToken,
            Created = _clock.UtcNow,
            Expires = _clock.UtcNow.Add(_tokenSettings.RefreshTokenLifetime)
        };

        var saveAction = new Func<ValueTask>(async () =>
        {
            await _refreshTokenRepository.WriteAsync(id, refreshTokenObject, true);
        });

        PendingActions.Enqueue(saveAction);

        return (accessToken, refreshToken);
    }

    public async ValueTask<bool> CheckRefreshToken(int id, string refreshToken)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var savedRefreshToken = await _refreshTokenRepository.ReadAsync(id, true);

        if (savedRefreshToken is null)
        {
            return false;
        }

        if (_clock.UtcNow > savedRefreshToken.Expires)
        {
            return false;
        }

        if (_clock.UtcNow < savedRefreshToken.Created)
        {
            throw new PersistifyInternalException();
        }

        return refreshToken == savedRefreshToken.Value;
    }

    public async ValueTask Update(User user)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (await _userRepository.ReadAsync(user.Id, true) is not null)
        {
            throw new PersistifyInternalException();
        }

        var updateAction = new Func<ValueTask>(async () =>
        {
            await _userRepository.WriteAsync(user.Id, user, true);
        });

        PendingActions.Enqueue(updateAction);
    }
}
