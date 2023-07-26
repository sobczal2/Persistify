using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Exceptions.Template;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

[StartupPriority(4)]
public class TemplateManager : ITemplateManager, IActOnStartup
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IDocumentIdManager _documentIdManager;
    private const string TemplateRepositoryKey = "Template/main";
    private const string DocumentRepositoryPrefix = "Document/template";

    private readonly SemaphoreSlim _generalTemplateLock;
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _individualSemaphores;

    private readonly ConcurrentDictionary<string, int> _templateNameToIdMap;
    private readonly ConcurrentDictionary<int, Template> _templates;
    private int _lastTemplateId;

    private IRepository<Template> TemplateRepository => _repositoryManager.Get<Template>(TemplateRepositoryKey);
    public TemplateManager(
        IRepositoryManager repositoryManager,
        IDocumentIdManager documentIdManager
    )
    {
        _repositoryManager = repositoryManager;
        _documentIdManager = documentIdManager;
        _individualSemaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
        _templates = new ConcurrentDictionary<int, Template>();
        _lastTemplateId = 0;
        _templateNameToIdMap = new ConcurrentDictionary<string, int>();
        _generalTemplateLock = new SemaphoreSlim(1, 1);

        repositoryManager.Create<Template>(TemplateRepositoryKey);
    }

    public async ValueTask CreateAsync(Template template)
    {
        int newId;
        await _generalTemplateLock.WaitAsync();
        try
        {
            if (_templateNameToIdMap.ContainsKey(template.Name))
            {
                throw new TemplateWithThatNameAlreadyExistsException();
            }

            newId = ++_lastTemplateId;

            if (!_templateNameToIdMap.TryAdd(template.Name, newId))
            {
                throw new ManagerInternalException($"Could not add template with name {template.Name}");
            }
        }
        finally
        {
            _generalTemplateLock.Release();
        }


        template.Id = newId;
        var newSemaphore = CreateIndividualSemaphore(newId);
        await newSemaphore.WaitAsync();
        try
        {
            await TemplateRepository.WriteAsync(newId, template);
            await _documentIdManager.InitializeForTemplate(newId);
            if (!_templates.TryAdd(newId, template))
            {
                throw new ManagerInternalException($"Could not add template with id {newId}");
            }

            _repositoryManager.Create<Document>(GetDocumentRepositoryKey(newId));
        }
        finally
        {
            newSemaphore.Release();
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

    public async ValueTask<Template> DeleteAsync(int id)
    {
        SemaphoreSlim? semaphore;
        await _generalTemplateLock.WaitAsync();
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
            semaphore = GetIndividualSemaphore(id);
            await semaphore.WaitAsync();
        }
        finally
        {
            _generalTemplateLock.Release();
        }

        try
        {
            await TemplateRepository.DeleteAsync(id);
            await _documentIdManager.RemoveForTemplate(id);
            if (!_templates.TryRemove(id, out var template))
            {
                throw new ManagerInternalException($"Could not remove template with id {id}");
            }

            _repositoryManager.Delete<Document>(GetDocumentRepositoryKey(id));

            return template;
        }
        finally
        {
            semaphore.Release();
            RemoveIndividualSemaphore(id);
        }
    }

    public async ValueTask PerformActionOnLockedTemplateAsync<TArgs>(int id, Func<Template, IRepository<Document>, TArgs, ValueTask> action, TArgs args, CancellationToken cancellationToken = default)
    {
        await _generalTemplateLock.WaitAsync(cancellationToken);
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
            _generalTemplateLock.Release();
        }
    }

    public async ValueTask<T> PerformActionOnLockedTemplateAsync<T, TArgs>(int id, Func<Template, IRepository<Document>, TArgs, ValueTask<T>> action, TArgs args, CancellationToken cancellationToken = default)
    {
        await _generalTemplateLock.WaitAsync(cancellationToken);
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
                return await action(_templates[id], _repositoryManager.Get<Document>(GetDocumentRepositoryKey(id)), args);
            }
            finally
            {
                semaphore.Release();
            }
        }
        finally
        {
            _generalTemplateLock.Release();
        }
    }

    // Not thread safe - should only be called once
    public async ValueTask PerformStartupActionAsync()
    {
        var templateIds = (await _documentIdManager.GetInitializedTemplates()).ToList();
        var templates = await TemplateRepository.ReadAllAsync().ToDictionaryAsync(x => x.Id, x => x);

        if (templateIds.Count != templates.Count)
        {
            throw new ManagerInternalException("Document ids and templates count mismatch");
        }

        for (var i = 0; i < templateIds.Count; i++)
        {
            var templateId = templateIds[i];
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
