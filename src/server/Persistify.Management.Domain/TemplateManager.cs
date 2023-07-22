using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.HostedServices;
using Persistify.Management.Domain.Abstractions;
using Persistify.Management.Domain.Exceptions;
using Persistify.Persistence.Core.Abstractions;

namespace Persistify.Management.Domain
{
    public class TemplateManager : ITemplateManager, IActOnStartup
    {
        private const string TemplateRepositoryKey = "TemplateRepository";
        private readonly IDocumentIdManager _documentIdManager;
        private readonly SemaphoreSlim _generalLock;
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks;
        private readonly IRepository<Template> _repository;
        private readonly ConcurrentDictionary<int, Template> _templates;

        public TemplateManager(
            IRepositoryFactory repositoryFactory,
            IDocumentIdManager documentIdManager
        )
        {
            _documentIdManager = documentIdManager;
            _templates = new ConcurrentDictionary<int, Template>();
            _repository = repositoryFactory.Create<Template>(TemplateRepositoryKey);
            _generalLock = new SemaphoreSlim(1, 1);
            _locks = new ConcurrentDictionary<int, SemaphoreSlim>();
        }

        public async ValueTask PerformStartupActionAsync()
        {
            await _generalLock.WaitAsync();
            try
            {
                _templates.Clear();
                _locks.Clear();

                var templates = _repository.ReadAllAsync();
                await foreach (var template in templates)
                {
                    if (!_templates.TryAdd(template.Id, template))
                    {
                        throw new TemplateManagerInternalException();
                    }
                }
            }
            finally
            {
                _generalLock.Release();
            }
        }

        public async ValueTask CreateAsync(Template template)
        {
            await _generalLock.WaitAsync();
            var id = _templates.Count + 1;
            var @lock = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await @lock.WaitAsync();
            try
            {
                template.Id = id;

                if (_templates.Values.Any(t => t.Name == template.Name))
                {
                    throw new TemplateNameAlreadyExistsException();
                }

                if (!_templates.TryAdd(id, template))
                {
                    throw new TemplateManagerInternalException();
                }

                await _repository.WriteAsync(id, template);
            }
            finally
            {
                @lock.Release();
                _generalLock.Release();
            }
        }

        public Template? Get(int id)
        {
            return _templates.TryGetValue(id, out var template) ? template : null;
        }

        public IEnumerable<Template> GetAll()
        {
            return _templates.Values;
        }

        public async ValueTask DeleteAsync(int id)
        {
            await _generalLock.WaitAsync();
            var @lock = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await @lock.WaitAsync();
            try
            {
                if (!_templates.TryRemove(id, out var template))
                {
                    throw new TemplateNotFoundException(id);
                }

                await _repository.RemoveAsync(id);
                await _documentIdManager.RemoveId(id);
            }
            finally
            {
                @lock.Release();
                _generalLock.Release();
            }
        }
    }
}
