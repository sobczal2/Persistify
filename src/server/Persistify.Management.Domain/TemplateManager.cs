using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.HostedServices;
using Persistify.Management.Domain.Abstractions;
using Persistify.Persistence.Core.Abstractions;

namespace Persistify.Management.Domain;

public class TemplateManager : ITemplateManager, IActOnStartup
{
    private readonly IDocumentIdManager _documentIdManager;
    private readonly IRepository<Template> _repository;
    private readonly ConcurrentDictionary<int, Template> _templates;
    private const string TemplateRepositoryKey = "TemplateRepository";
    private readonly ReaderWriterLockSlim _generalLock;
    private readonly ConcurrentDictionary<int, ReaderWriterLockSlim> _locks;

    public TemplateManager(
        IRepositoryFactory repositoryFactory,
        IDocumentIdManager documentIdManager
    )
    {
        _documentIdManager = documentIdManager;
        _templates = new ConcurrentDictionary<int, Template>();
        _repository = repositoryFactory.Create<Template>(TemplateRepositoryKey);
        _generalLock = new ReaderWriterLockSlim();
        _locks = new ConcurrentDictionary<int, ReaderWriterLockSlim>();
    }

    public async ValueTask CreateAsync(Template template)
    {
        _generalLock.EnterWriteLock();
        try
        {
            var id = _templates.Count + 1;
            template.Id = id;
            _templates.TryAdd(id, template);
            await _repository.WriteAsync(id, template);
        }
        finally
        {
            _generalLock.ExitWriteLock();
        }
    }

    public ValueTask<Template?> GetAsync(int id)
    {
        var @lock = _locks.GetOrAdd(id, _ => new ReaderWriterLockSlim());
        try
        {
            return ValueTask.FromResult(_templates.TryGetValue(id, out var template) ? template : null);
        }
        finally
        {
            @lock.ExitReadLock();
        }
    }

    public ValueTask<IEnumerable<Template>> GetAllAsync()
    {
        _generalLock.EnterReadLock();
        try
        {
            return ValueTask.FromResult(_templates.Values.AsEnumerable());
        }
        finally
        {
            _generalLock.ExitReadLock();
        }
    }

    public async ValueTask DeleteAsync(int id)
    {
        _generalLock.EnterWriteLock();
        var @lock = _locks.GetOrAdd(id, _ => new ReaderWriterLockSlim());
        try
        {
            _templates.TryRemove(id, out _);
            await _repository.RemoveAsync(id);
            await _documentIdManager.RemoveId(id);
        }
        finally
        {
            _generalLock.ExitWriteLock();
            @lock.ExitWriteLock();
        }
    }

    public ValueTask AcquireReadLockAsync(int id)
    {
        var @lock = _locks.GetOrAdd(id, _ => new ReaderWriterLockSlim());
        @lock.EnterReadLock();
        return ValueTask.CompletedTask;
    }

    public ValueTask ReleaseReadLockAsync(int id)
    {
        var @lock = _locks.GetOrAdd(id, _ => new ReaderWriterLockSlim());
        @lock.ExitReadLock();
        return ValueTask.CompletedTask;
    }

    public async ValueTask PerformStartupActionAsync()
    {
        _templates.Clear();
        var templates = await _repository.ReadAllAsync();
        foreach (var template in templates)
        {
            _templates.TryAdd(template.Id, template);
        }
    }
}
