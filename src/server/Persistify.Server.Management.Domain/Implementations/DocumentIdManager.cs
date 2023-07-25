using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions.DocumentId;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Implementations;

public class DocumentIdManager : IDocumentIdManager, IActOnStartup
{
    private readonly ILinearRepositoryManager _linearRepositoryManager;
    private const string DocumentIdKey = "DocumentId/main";
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ISet<int> _initializedTemplates;

    public DocumentIdManager(
        ILinearRepositoryManager linearRepositoryManager
    )
    {
        _linearRepositoryManager = linearRepositoryManager;
        linearRepositoryManager.Create(DocumentIdKey);
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _initializedTemplates = new HashSet<int>();
    }

    public async ValueTask<long> GetNextId(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var repository = _linearRepositoryManager.Get(DocumentIdKey);
            if (!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

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

    public async ValueTask<long> GetCurrentId(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

            var currentId = await _linearRepositoryManager.Get(DocumentIdKey).ReadAsync(templateId);

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

    public async ValueTask InitializeForTemplate(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(_initializedTemplates.Contains(templateId))
            {
                throw new TemplateAlreadyInitializedException();
            }

            await _linearRepositoryManager.Get(DocumentIdKey).WriteAsync(templateId, 0);
            _initializedTemplates.Add(templateId);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask RemoveForTemplate(int templateId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if(!_initializedTemplates.Contains(templateId))
            {
                throw new TemplateNotInitializedException();
            }

            await _linearRepositoryManager.Get(DocumentIdKey).RemoveAsync(templateId);
            _initializedTemplates.Remove(templateId);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<IEnumerable<int>> GetInitializedTemplates()
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

    public async ValueTask PerformStartupActionAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var kv = await _linearRepositoryManager.Get(DocumentIdKey).ReadAllAsync();
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
}
