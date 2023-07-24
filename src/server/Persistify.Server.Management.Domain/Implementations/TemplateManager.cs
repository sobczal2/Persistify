﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Management.Domain.Exceptions.DocumentId;
using Persistify.Server.Management.Domain.Exceptions.Template;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Implementations;

[HasStartupDependencyOn(typeof(DocumentIdManager))]
public class TemplateManager : ITemplateManager, IActOnStartup
{
    private readonly IDocumentIdManager _documentIdManager;
    private const string TemplateRepositoryKey = "Template/main";

    private SpinLock _lastTemplateIdLock;
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _individualSemaphores;

    private readonly ConcurrentDictionary<string, int> _templateNameToIdMap;
    private readonly ConcurrentDictionary<int, Template> _templates;
    private int _lastTemplateId;

    private readonly IRepository<Template> _templateRepository;

    public TemplateManager(
        IRepositoryFactory repositoryFactory,
        IDocumentIdManager documentIdManager
    )
    {
        _documentIdManager = documentIdManager;
        _individualSemaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
        _templates = new ConcurrentDictionary<int, Template>();
        _lastTemplateId = 0;
        _templateRepository = repositoryFactory.Create<Template>(TemplateRepositoryKey);
        _templateNameToIdMap = new ConcurrentDictionary<string, int>();
        _lastTemplateIdLock = new SpinLock();
    }

    public async ValueTask CreateAsync(Template template)
    {
        int newId;
        var lockTaken = false;
        _lastTemplateIdLock.Enter(ref lockTaken);
        try
        {
            if (_templateNameToIdMap.ContainsKey(template.Name))
            {
                throw new TemplateWithThatNameAlreadyExistsException();
            }

            newId = ++_lastTemplateId;

            if(!_templateNameToIdMap.TryAdd(template.Name, newId))
            {
                throw new ManagerInternalException($"Could not add template with name {template.Name}");
            }
        }
        finally
        {
            if (lockTaken)
            {
                _lastTemplateIdLock.Exit();
            }
        }



        template.Id = newId;
        var newSemaphore = CreateIndividualSemaphore(newId);
        await newSemaphore.WaitAsync();
        try
        {
            await _templateRepository.WriteAsync(newId, template);
            await _documentIdManager.InitializeForTemplate(newId);
            if(!_templates.TryAdd(newId, template))
            {
                throw new ManagerInternalException($"Could not add template with id {newId}");
            }
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

    public async ValueTask DeleteAsync(int id)
    {
        var semaphore = GetIndividualSemaphore(id);
        await semaphore.WaitAsync();
        var lockTaken = false;
        _lastTemplateIdLock.Enter(ref lockTaken);
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
        }
        finally
        {
            if (lockTaken)
            {
                _lastTemplateIdLock.Exit();
            }
        }

        try
        {
            await _templateRepository.DeleteAsync(id);
            await _documentIdManager.RemoveForTemplate(id);
            if(!_templates.TryRemove(id, out _))
            {
                throw new ManagerInternalException($"Could not remove template with id {id}");
            }
        }
        finally
        {
            semaphore.Release();
            RemoveIndividualSemaphore(id);
        }
    }

    public async ValueTask LockTemplateAsync(int id, CancellationToken cancellationToken = default)
    {
        var semaphore = GetIndividualSemaphore(id);
        await semaphore.WaitAsync(cancellationToken);
    }

    public void UnlockTemplate(int id)
    {
        var semaphore = GetIndividualSemaphore(id);
        semaphore.Release();
    }

    // Not thread safe - should only be called once
    public async ValueTask PerformStartupActionAsync()
    {
        var documentIds = (await _documentIdManager.GetInitializedTemplates()).ToList();
        var templates = await _templateRepository.ReadAllAsync().ToDictionaryAsync(x => x.Id, x => x);

        if(documentIds.Count != templates.Count)
        {
            throw new ManagerInternalException("Document ids and templates count mismatch");
        }

        for (var i = 0; i < documentIds.Count; i++)
        {
            var documentId = documentIds[i];
            var template = templates[documentId];
            CreateIndividualSemaphore(documentId);
            if(!_templates.TryAdd(documentId, template))
            {
                throw new ManagerInternalException($"Could not add template with id {documentId}");
            }

            if(!_templateNameToIdMap.TryAdd(template.Name, documentId))
            {
                throw new ManagerInternalException($"Could not add template with name {template.Name}");
            }

            if(template.Id > _lastTemplateId)
            {
                _lastTemplateId = template.Id;
            }
        }
    }

    private SemaphoreSlim CreateIndividualSemaphore(int id)
    {
        if(_individualSemaphores.ContainsKey(id))
        {
            throw new ManagerInternalException($"Semaphore for template with id {id} already exists");
        }

        var newSemaphore = new SemaphoreSlim(1, 1);
        if(!_individualSemaphores.TryAdd(id, newSemaphore))
        {
            throw new ManagerInternalException($"Could not add semaphore for template with id {id}");
        }

        return newSemaphore;
    }

    private SemaphoreSlim GetIndividualSemaphore(int id)
    {
        if(!_individualSemaphores.TryGetValue(id, out var semaphore))
        {
            throw new ManagerInternalException($"Could not get semaphore for template with id {id}");
        }

        return semaphore;
    }

    private void RemoveIndividualSemaphore(int id)
    {
        if(!_individualSemaphores.TryRemove(id, out _))
        {
            throw new ManagerInternalException($"Could not remove semaphore for template with id {id}");
        }
    }
}
