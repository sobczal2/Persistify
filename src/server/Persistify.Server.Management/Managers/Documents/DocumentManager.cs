using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Documents;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Transactions.Exceptions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManager : Manager, IDocumentManager
{
    private readonly int _templateId;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<Document> _documentRepository;

    public DocumentManager(
        int templateId,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    )
    {
        _templateId = templateId;

        var identifierFileStream =
            fileStreamFactory.CreateStream(DocumentManagerFileGroupForTemplate.IdentifierFileName(_templateId));
        var documentRepositoryMainFileStream =
            fileStreamFactory.CreateStream(DocumentManagerFileGroupForTemplate.DocumentRepositoryMainFileName(_templateId));
        var documentRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(DocumentManagerFileGroupForTemplate.DocumentRepositoryOffsetLengthFileName(_templateId));

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _documentRepository = new ObjectStreamRepository<Document>(
            documentRepositoryMainFileStream,
            documentRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.DocumentRepositorySectorSize
        );
    }

    public async ValueTask<Document?> GetAsync(int id)
    {
        if (!CanRead())
        {
            throw new NotAllowedForTransactionException();
        }

        return await _documentRepository.ReadAsync(id, true);
    }

    public async ValueTask<List<Document>> ListAsync(int take, int skip)
    {
        if (!CanRead())
        {
            throw new NotAllowedForTransactionException();
        }

        var kvList = await _documentRepository.ReadRangeAsync(take, skip, true);
        var list = new List<Document>(kvList.Count);

        foreach (var (key, value) in kvList)
        {
            list.Add(value);
        }

        return list;
    }

    public async ValueTask<int> CountAsync()
    {
        if (!CanRead())
        {
            throw new NotAllowedForTransactionException();
        }

        return await _documentRepository.CountAsync(true);
    }

    public void Add(Document document)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            // TODO: Do this on initialization
            if (_identifierRepository.IsValueEmpty(currentId))
            {
                currentId = 0;
            }
            else
            {
                currentId++;
            }

            await _identifierRepository.WriteAsync(0, currentId, true);

            await _documentRepository.WriteAsync(currentId, document, true);

            document.Id = currentId;
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        if (!await _documentRepository.ExistsAsync(id, true))
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            await _documentRepository.DeleteAsync(id, true);
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }
}
