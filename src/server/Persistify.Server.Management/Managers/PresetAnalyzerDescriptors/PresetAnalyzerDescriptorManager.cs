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

public class PresetAnalyzerDescriptorManager : Manager, IPresetAnalyzerDescriptorManager
{
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<PresetAnalyzerDescriptor> _presetAnalyzerDescriptorRepository;
    private readonly ConcurrentDictionary<string, int> _presetAnalyzerDescriptorNameIdDictionary;
    private volatile int _count;

    public PresetAnalyzerDescriptorManager(
        ITransactionState transactionState,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        var identifierFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerDescriptorManagerRequiredFileGroup.IdentifierRepositoryFileName);
        var presetAnalyzerDescriptorRepositoryMainFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerDescriptorManagerRequiredFileGroup.TemplateRepositoryMainFileName);
        var presetAnalyzerDescriptorRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(PresetAnalyzerDescriptorManagerRequiredFileGroup.TemplateRepositoryOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _presetAnalyzerDescriptorRepository = new ObjectStreamRepository<PresetAnalyzerDescriptor>(
            presetAnalyzerDescriptorRepositoryMainFileStream,
            presetAnalyzerDescriptorRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.PresetAnalyzerDescriptorRepositorySectorSize
        );

        _presetAnalyzerDescriptorNameIdDictionary = new ConcurrentDictionary<string, int>();
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

            await foreach (var (key, presetAnalyzerDescriptor) in _presetAnalyzerDescriptorRepository.ReadAllAsync(true))
            {
                _presetAnalyzerDescriptorNameIdDictionary.TryAdd(presetAnalyzerDescriptor.Name, key);

                Interlocked.Increment(ref _count);
            }

            base.Initialize();
        });

        PendingActions.Enqueue(initializeAction);
    }

    public async ValueTask<PresetAnalyzerDescriptor?> GetAsync(string presetName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        if (!_presetAnalyzerDescriptorNameIdDictionary.TryGetValue(presetName, out var id))
        {
            return null;
        }

        return await _presetAnalyzerDescriptorRepository.ReadAsync(id, true);
    }

    public bool Exists(string presetName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _presetAnalyzerDescriptorNameIdDictionary.ContainsKey(presetName);
    }

    public async IAsyncEnumerable<PresetAnalyzerDescriptor> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        await foreach (var (_, template) in _presetAnalyzerDescriptorRepository.ReadRangeAsync(take, skip, true))
        {
            yield return template;
        }
    }

    public void Add(PresetAnalyzerDescriptor preset)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (_presetAnalyzerDescriptorNameIdDictionary.ContainsKey(preset.Name))
        {
            throw new InternalPersistifyException();
        }

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            currentId++;

            await _identifierRepository.WriteAsync(0, currentId, true);

            preset.Id = currentId;

            await _presetAnalyzerDescriptorRepository.WriteAsync(currentId, preset, true);

            Interlocked.Increment(ref _count);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(string presetName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (!_presetAnalyzerDescriptorNameIdDictionary.TryGetValue(presetName, out var id))
        {
            return false;
        }

        var presetAnalyzerDescriptor = await _presetAnalyzerDescriptorRepository.ReadAsync(id, true);

        if (presetAnalyzerDescriptor is null)
        {
            throw new InternalPersistifyException();
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            if (await _presetAnalyzerDescriptorRepository.DeleteAsync(id, true))
            {
                if(!_presetAnalyzerDescriptorNameIdDictionary.TryRemove(presetAnalyzerDescriptor.Name, out _))
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
