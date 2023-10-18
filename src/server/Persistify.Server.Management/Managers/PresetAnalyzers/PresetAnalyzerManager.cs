using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.PresetAnalyzerDescriptors;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Files;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Persistence.Extensions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;

public class PresetAnalyzerManager : Manager, IPresetAnalyzerManager
{
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<PresetAnalyzer> _presetAnalyzerRepository;
    private readonly ConcurrentDictionary<string, int> _presetAnalyzerNameIdDictionary;
    private volatile int _count;

    public PresetAnalyzerManager(
        ITransactionState transactionState,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        var identifierFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerManagerRequiredFileGroup.IdentifierRepositoryFileName);
        var presetAnalyzerRepositoryMainFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerManagerRequiredFileGroup.PresetAnalyzerRepositoryMainFileName);
        var presetAnalyzerRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerManagerRequiredFileGroup.PresetAnalyzerRepositoryOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _presetAnalyzerRepository = new ObjectStreamRepository<PresetAnalyzer>(
            presetAnalyzerRepositoryMainFileStream,
            presetAnalyzerRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.PresetAnalyzerDescriptorRepositorySectorSize
        );

        _presetAnalyzerNameIdDictionary = new ConcurrentDictionary<string, int>();
        _count = 0;
    }

    public override string Name => "AnalyzerDescriptorManager";

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

            _count = 0;

            await foreach (var (key, presetAnalyzer) in _presetAnalyzerRepository.ReadAllAsync(true))
            {
                _presetAnalyzerNameIdDictionary.TryAdd(presetAnalyzer.Name, key);

                Interlocked.Increment(ref _count);
            }

            base.Initialize();
        });

        PendingActions.Enqueue(initializeAction);
    }

    public async ValueTask<PresetAnalyzer?> GetAsync(string presetAnalyzerName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        if (!_presetAnalyzerNameIdDictionary.TryGetValue(presetAnalyzerName, out var id))
        {
            return null;
        }

        return await _presetAnalyzerRepository.ReadAsync(id, true);
    }

    public bool Exists(string presetName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _presetAnalyzerNameIdDictionary.ContainsKey(presetName);
    }

    public async IAsyncEnumerable<PresetAnalyzer> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        await foreach (var (_, template) in _presetAnalyzerRepository.ReadRangeAsync(take, skip, true))
        {
            yield return template;
        }
    }

    public void Add(PresetAnalyzer presetAnalyzer)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (_presetAnalyzerNameIdDictionary.ContainsKey(presetAnalyzer.Name))
        {
            throw new InternalPersistifyException();
        }

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            currentId++;

            await _identifierRepository.WriteAsync(0, currentId, true);

            presetAnalyzer.Id = currentId;

            await _presetAnalyzerRepository.WriteAsync(currentId, presetAnalyzer, true);

            _presetAnalyzerNameIdDictionary.TryAdd(presetAnalyzer.Name, currentId);

            Interlocked.Increment(ref _count);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(string presetAnalyzerName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (!_presetAnalyzerNameIdDictionary.TryGetValue(presetAnalyzerName, out var id))
        {
            return false;
        }

        var presetAnalyzer = await _presetAnalyzerRepository.ReadAsync(id, true);

        if (presetAnalyzer is null)
        {
            throw new InternalPersistifyException();
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            if (await _presetAnalyzerRepository.DeleteAsync(id, true))
            {
                if(!_presetAnalyzerNameIdDictionary.TryRemove(presetAnalyzer.Name, out _))
                {
                    throw new InternalPersistifyException();
                }

                Interlocked.Decrement(ref _count);
            }
            else
            {
                throw new InternalPersistifyException();
            }
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }
}
