using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Users;

public class UserManager : Manager, IUserManager
{
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<User> _userRepository;
    private readonly ConcurrentDictionary<string, int> _usernameIdDictionary;
    private volatile int _count;

    public UserManager(
        ITransactionState transactionState,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        var identifierFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.IdentifierRepositoryFileName);
        var userRepositoryMainFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.UserRepositoryMainFileName);
        var userRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(UserManagerRequiredFileGroup.UserRepositoryOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _userRepository = new ObjectStreamRepository<User>(
            userRepositoryMainFileStream,
            userRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.UserRepositorySectorSize
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

            Interlocked.Increment(ref _count);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (!await _userRepository.ExistsAsync(id, true))
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            if (await _userRepository.DeleteAsync(id, true))
            {
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
}
