using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Documents;

public class DocumentRepositoryManager : IDocumentRepositoryManager
{
    private readonly ConcurrentDictionary<int, IDocumentRepository> _repositories;

    public DocumentRepositoryManager()
    {
        _repositories = new ConcurrentDictionary<int, IDocumentRepository>();
    }

    public IDocumentRepository GetRepository(int templateId)
    {
        if (_repositories.TryGetValue(templateId, out var repository))
        {
            return repository;
        }

        throw new KeyNotFoundException();
    }

    public void AddRepository(int templateId)
    {
        var repository = new DocumentRepository();

        if (!_repositories.TryAdd(templateId, repository))
        {
            throw new InvalidOperationException();
        }
    }

    public void DeleteRepository(int templateId)
    {
        if (!_repositories.TryRemove(templateId, out _))
        {
            throw new KeyNotFoundException();
        }
    }
}
