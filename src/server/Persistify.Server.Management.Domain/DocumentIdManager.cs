using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Exceptions.DocumentId;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

[StartupPriority(5)]
public class DocumentIdManager : IDocumentIdManager, IActOnStartup, IDisposable
{
    private readonly IIntLinearRepositoryManager _intLinearRepositoryManager;
    private const string DocumentIdKey = "DocumentId/main";
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ISet<int> _initializedTemplates;

    public DocumentIdManager(
        IIntLinearRepositoryManager intLinearRepositoryManager
    )
    {
        _intLinearRepositoryManager = intLinearRepositoryManager;
        intLinearRepositoryManager.Create(DocumentIdKey);
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _initializedTemplates = new HashSet<int>();
    }

    public async ValueTask<long> GetNextIdAsync(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

            var repository = _intLinearRepositoryManager.Get(DocumentIdKey);
            var currentId = await repository.ReadAsync(templateId);

            if (currentId is null)
            {
                throw new ManagerInternalException("Current id is null");
            }

            var nextId = currentId.Value + 1;
            await repository.WriteAsync(templateId, nextId);
            return nextId;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<long> GetCurrentIdAsync(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

            var currentId = await _intLinearRepositoryManager.Get(DocumentIdKey).ReadAsync(templateId);

            if(currentId is null)
            {
                throw new ManagerInternalException("Current id is null");
            }

            return currentId.Value;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask InitializeForTemplateAsync(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(_initializedTemplates.Contains(templateId))
            {
                throw new TemplateAlreadyInitializedException();
            }

            await _intLinearRepositoryManager.Get(DocumentIdKey).WriteAsync(templateId, 0);
            _initializedTemplates.Add(templateId);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask RemoveForTemplateAsync(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

            await _intLinearRepositoryManager.Get(DocumentIdKey).RemoveAsync(templateId);
            _initializedTemplates.Remove(templateId);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<IEnumerable<int>> GetInitializedTemplatesAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            return _initializedTemplates.ToList();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<bool> ExistsForTemplateAsync(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            return _initializedTemplates.Contains(templateId);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask PerformStartupActionAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var kv = await _intLinearRepositoryManager.Get(DocumentIdKey).ReadAllAsync();
            foreach(var (templateId, _) in kv)
            {
                if(templateId < 1)
                {
                    throw new ManagerInternalException("Template id is less than 1");
                }

                if(templateId > int.MaxValue)
                {
                    throw new ManagerInternalException("Template id is greater than int.MaxValue");
                }

                var intTemplateId = (int)templateId;

                if(_initializedTemplates.Contains(intTemplateId))
                {
                    throw new ManagerInternalException("Template id is already initialized");
                }

                _initializedTemplates.Add(intTemplateId);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }
}
