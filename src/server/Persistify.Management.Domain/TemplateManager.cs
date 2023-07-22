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
    private readonly Mutex _mutex;

    public TemplateManager(
        IRepositoryFactory repositoryFactory,
        IDocumentIdManager documentIdManager
    )
    {
        _documentIdManager = documentIdManager;
        _templates = new ConcurrentDictionary<int, Template>();
        _repository = repositoryFactory.Create<Template>(TemplateRepositoryKey);
        _mutex = new Mutex();
    }

    public async ValueTask CreateAsync(Template template)
    {
        _mutex.WaitOne();
        try
        {
            var id = _templates.Count + 1;
            template.Id = id;
            _templates.TryAdd(id, template);
            await _repository.WriteAsync(id, template);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public ValueTask<Template?> GetAsync(int id)
    {
        return ValueTask.FromResult(_templates.GetValueOrDefault(id));
    }

    public ValueTask<IEnumerable<Template>> GetAllAsync()
    {
        return ValueTask.FromResult(_templates.Values.AsEnumerable());
    }

    public async ValueTask DeleteAsync(int id)
    {
        _mutex.WaitOne();
        try
        {
            _templates.TryRemove(id, out _);
            await _repository.RemoveAsync(id);
            await _documentIdManager.RemoveId(id);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
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
