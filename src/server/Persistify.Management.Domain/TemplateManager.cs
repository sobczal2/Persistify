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
        private readonly SemaphoreSlim _generalSemaphore;
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _semaphores;
        private readonly IRepository<Template> _repository;
        private readonly ConcurrentDictionary<int, Template> _templates;
        private readonly ConcurrentDictionary<string, int> _templateNameIdMap;

        public TemplateManager(
            IRepositoryFactory repositoryFactory,
            IDocumentIdManager documentIdManager
        )
        {
            _documentIdManager = documentIdManager;
            _templates = new ConcurrentDictionary<int, Template>();
            _repository = repositoryFactory.Create<Template>(TemplateRepositoryKey);
            _generalSemaphore = new SemaphoreSlim(1, 1);
            _semaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
            _templateNameIdMap = new ConcurrentDictionary<string, int>();
        }

        public async ValueTask PerformStartupActionAsync()
        {
            await _generalSemaphore.WaitAsync();
            try
            {
                _templates.Clear();
                _semaphores.Clear();

                var templates = _repository.ReadAllAsync();
                await foreach (var template in templates)
                {
                    if (!_templates.TryAdd(template.Id, template))
                    {
                        throw new TemplateManagerInternalException();
                    }

                    if (!_templateNameIdMap.TryAdd(template.Name, template.Id))
                    {
                        throw new TemplateManagerInternalException();
                    }
                }
            }
            finally
            {
                _generalSemaphore.Release();
            }
        }

        public async ValueTask CreateAsync(Template template)
        {
            await _generalSemaphore.WaitAsync();
            var id = _templates.Count + 1;
            var @lock = _semaphores.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await @lock.WaitAsync();
            try
            {
                template.Id = id;

                if (!_templateNameIdMap.TryAdd(template.Name, id))
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
                _generalSemaphore.Release();
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
            await _generalSemaphore.WaitAsync();
            var @lock = _semaphores.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await @lock.WaitAsync();
            try
            {
                if (!_templates.TryRemove(id, out var template))
                {
                    throw new TemplateNotFoundException(id);
                }

                if (!_templateNameIdMap.TryRemove(template.Name, out _))
                {
                    throw new TemplateManagerInternalException();
                }

                await _repository.RemoveAsync(id);
                await _documentIdManager.RemoveId(id);
            }
            finally
            {
                @lock.Release();
                _generalSemaphore.Release();
            }
        }
    }
}
