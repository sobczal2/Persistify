using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Exceptions.Template;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

[StartupPriority(4)]
public class TemplateManager : ITemplateManager, IActOnStartup
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IDocumentIdManager _documentIdManager;
    private readonly ILogger<TemplateManager> _logger;
    private const string TemplateRepositoryKey = "Template/main";
    private const string DocumentRepositoryPrefix = "Document/template";

    private readonly SemaphoreSlim _generalTemplateSemaphore;
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _individualSemaphores;

    private readonly ConcurrentDictionary<string, int> _templateNameToIdMap;
    private readonly ConcurrentDictionary<int, Template> _templates;
    private int _lastTemplateId;
    private readonly IRepository<Template> _repository;

    public TemplateManager(
        IRepositoryManager repositoryManager,
        IDocumentIdManager documentIdManager,
        ILogger<TemplateManager> logger
    )
    {
        _repositoryManager = repositoryManager;
        _documentIdManager = documentIdManager;
        _logger = logger;
        _individualSemaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
        _templates = new ConcurrentDictionary<int, Template>();
        _lastTemplateId = 0;
        _templateNameToIdMap = new ConcurrentDictionary<string, int>();
        _generalTemplateSemaphore = new SemaphoreSlim(1, 1);
        repositoryManager.Create<Template>(TemplateRepositoryKey);
        _repository = repositoryManager.Get<Template>(TemplateRepositoryKey);
    }

    // TODO: Revisit this method in terms of fault tolerance
    public async ValueTask CreateAsync(Template template)
    {
        await _generalTemplateSemaphore.WaitAsync();
        int startingId = _lastTemplateId;
        try
        {
            if (_templateNameToIdMap.ContainsKey(template.Name))
            {
                throw new TemplateWithThatNameAlreadyExistsException();
            }

            var newId = _lastTemplateId + 1;
            _templateNameToIdMap[template.Name] = newId;

            template.Id = newId;
            var newSemaphore = CreateIndividualSemaphore(newId);
            await newSemaphore.WaitAsync();
            try
            {
                await _repository.WriteAsync(newId, template);
                await _documentIdManager.InitializeForTemplateAsync(newId);
                if (!_templates.TryAdd(newId, template))
                {
                    throw new ManagerInternalException($"Could not add template with id {newId}");
                }

                _repositoryManager.Create<Document>(GetDocumentRepositoryKey(newId));
                _lastTemplateId = newId;
            }
            catch (Exception)
            {
                _lastTemplateId = startingId;
                _templateNameToIdMap.TryRemove(template.Name, out _);
                _individualSemaphores.TryRemove(template.Id, out _);
                if (await _repository.ExistsAsync(template.Id))
                {
                    await _repository.DeleteAsync(template.Id);
                }

                if (await _documentIdManager.ExistsForTemplateAsync(template.Id))
                {
                    await _documentIdManager.RemoveForTemplateAsync(template.Id);
                }

                _templates.TryRemove(template.Id, out _);
                if (_repositoryManager.Exists<Template>(TemplateRepositoryKey))
                {
                    _repositoryManager.Delete<Document>(GetDocumentRepositoryKey(template.Id));
                }

                template.Id = 0;
                throw;
            }
            finally
            {
                newSemaphore.Release();
            }
        }
        finally
        {
            _generalTemplateSemaphore.Release();
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

    // TODO: Revisit this method in terms of fault tolerance
    public async ValueTask<Template> DeleteAsync(int id)
    {
        Template? template = null;
        await _generalTemplateSemaphore.WaitAsync();
        try
        {
            if (!_templates.ContainsKey(id))
            {
                throw new TemplateNotFoundException(id);
            }

            if (!_templateNameToIdMap.TryRemove(_templates[id].Name, out _))
            {
                throw new ManagerInternalException($"Could not remove template with name {_templates[id].Name}");
            }

            var semaphore = GetIndividualSemaphore(id);
            await semaphore.WaitAsync();
            try
            {
                if (!_templates.TryRemove(id, out template))
                {
                    throw new ManagerInternalException($"Could not remove template with id {id}");
                }
                await _repository.DeleteAsync(id);
                await _documentIdManager.RemoveForTemplateAsync(id);

                _repositoryManager.Delete<Document>(GetDocumentRepositoryKey(id));
                RemoveIndividualSemaphore(id);

                semaphore.Release();
                semaphore.Dispose();

                return template;
            }
            catch (Exception)
            {
                if (template is null)
                {
                    throw new ManagerInternalException(
                        $"Fatal error while deleting template with id {id}. Data may be corrupted");
                }

                _templateNameToIdMap.TryAdd(template.Name, id);
                if (!await _repository.ExistsAsync(id))
                {
                    await _repository.WriteAsync(id, template);
                }

                if (!await _documentIdManager.ExistsForTemplateAsync(id))
                {
                    await _documentIdManager.InitializeForTemplateAsync(id);
                }

                _templates.TryAdd(id, template);
                if (!_repositoryManager.Exists<Document>(GetDocumentRepositoryKey(id)))
                {
                    _logger.LogError("Already deleted document repository for template with id {TemplateId}. Data loss has occurred", id);
                    _repositoryManager.Create<Document>(GetDocumentRepositoryKey(id));
                }

                if (!_individualSemaphores.ContainsKey(id))
                {
                    CreateIndividualSemaphore(id);
                }

                semaphore.Release();
                throw;
            }
        }
        finally
        {
            _generalTemplateSemaphore.Release();
        }
    }

    public async ValueTask PerformActionOnLockedTemplateAsync<TArgs>(int id,
        Func<Template, IRepository<Document>, TArgs, ValueTask> action, TArgs args,
        CancellationToken cancellationToken = default)
    {
        await _generalTemplateSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!_templates.ContainsKey(id))
            {
                throw new TemplateNotFoundException(id);
            }

            var semaphore = GetIndividualSemaphore(id);
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                await action(_templates[id], _repositoryManager.Get<Document>(GetDocumentRepositoryKey(id)), args);
            }
            finally
            {
                semaphore.Release();
            }
        }
        finally
        {
            _generalTemplateSemaphore.Release();
        }
    }

    public async ValueTask<T> PerformActionOnLockedTemplateAsync<T, TArgs>(int id,
        Func<Template, IRepository<Document>, TArgs, ValueTask<T>> action, TArgs args,
        CancellationToken cancellationToken = default)
    {
        await _generalTemplateSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!_templates.ContainsKey(id))
            {
                throw new TemplateNotFoundException(id);
            }

            var semaphore = GetIndividualSemaphore(id);
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                return await action(_templates[id], _repositoryManager.Get<Document>(GetDocumentRepositoryKey(id)),
                    args);
            }
            finally
            {
                semaphore.Release();
            }
        }
        finally
        {
            _generalTemplateSemaphore.Release();
        }
    }

    public async ValueTask PerformStartupActionAsync()
    {
        await _generalTemplateSemaphore.WaitAsync();
        try
        {
            var templateIds = (await _documentIdManager.GetInitializedTemplatesAsync()).ToList();
            var templates = (await _repository.ReadAllAsync()).ToDictionary(x => x.Id, x => x);

            if (templateIds.Count != templates.Count)
            {
                throw new ManagerInternalException("Document ids and templates count mismatch");
            }

            foreach (var templateId in templateIds)
            {
                var template = templates[templateId];
                CreateIndividualSemaphore(templateId);
                if (!_templates.TryAdd(templateId, template))
                {
                    throw new ManagerInternalException($"Could not add template with id {templateId}");
                }

                if (!_templateNameToIdMap.TryAdd(template.Name, templateId))
                {
                    throw new ManagerInternalException($"Could not add template with name {template.Name}");
                }

                if (template.Id > _lastTemplateId)
                {
                    _lastTemplateId = template.Id;
                }

                _repositoryManager.Create<Document>(GetDocumentRepositoryKey(templateId));
            }
        }
        finally
        {
            _generalTemplateSemaphore.Release();
        }
    }

    private SemaphoreSlim CreateIndividualSemaphore(int id)
    {
        if (_individualSemaphores.ContainsKey(id))
        {
            throw new ManagerInternalException($"Semaphore for template with id {id} already exists");
        }

        var newSemaphore = new SemaphoreSlim(1, 1);
        if (!_individualSemaphores.TryAdd(id, newSemaphore))
        {
            throw new ManagerInternalException($"Could not add semaphore for template with id {id}");
        }

        return newSemaphore;
    }

    private SemaphoreSlim GetIndividualSemaphore(int id)
    {
        if (!_individualSemaphores.TryGetValue(id, out var semaphore))
        {
            throw new ManagerInternalException($"Could not get semaphore for template with id {id}");
        }

        return semaphore;
    }

    private void RemoveIndividualSemaphore(int id)
    {
        if (!_individualSemaphores.TryRemove(id, out _))
        {
            throw new ManagerInternalException($"Could not remove semaphore for template with id {id}");
        }
    }

    private string GetDocumentRepositoryKey(int templateId)
    {
        return $"{DocumentRepositoryPrefix}{templateId:X}";
    }
}
