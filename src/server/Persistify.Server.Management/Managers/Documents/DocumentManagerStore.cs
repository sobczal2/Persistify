using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManagerStore : IDocumentManagerStore
{
    private readonly ConcurrentDictionary<int, IDocumentManager> _repositories;

    public DocumentManagerStore()
    {
        _repositories = new ConcurrentDictionary<int, IDocumentManager>();
    }

    public IDocumentManager GetManager(int templateId)
    {
        if (_repositories.TryGetValue(templateId, out var repository))
        {
            return repository;
        }

        throw new KeyNotFoundException();
    }

    public void AddManager(int templateId)
    {
        var repository = new DocumentManager();

        if (!_repositories.TryAdd(templateId, repository))
        {
            throw new InvalidOperationException();
        }
    }

    public void DeleteManager(int templateId)
    {
        if (!_repositories.TryRemove(templateId, out _))
        {
            throw new KeyNotFoundException();
        }
    }
}
