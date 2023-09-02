﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions.Exceptions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Templates;

public class TemplateManager : Manager, ITemplateManager
{
    private readonly IFileManager _fileManager;
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<Template> _templateRepository;

    public TemplateManager(
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IFileManager fileManager,
        IDocumentManagerStore documentManagerStore
    )
    {
        _fileManager = fileManager;
        _documentManagerStore = documentManagerStore;
        var identifierFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.IdentifierFileName);
        var innerTemplateMainFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.InnerTemplateMainFileName);
        var innerTemplateOffsetLengthFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.InnerTemplateOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _templateRepository = new ObjectStreamRepository<Template>(
            innerTemplateMainFileStream,
            innerTemplateOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.TemplateRepositorySectorSize
        );
    }

    public async ValueTask<Template?> GetAsync(int id)
    {
        if (!CanRead())
        {
            throw new NotAllowedForTransactionException();
        }

        return await _templateRepository.ReadAsync(id, true);
    }

    public void Add(Template template)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);
            if (_identifierRepository.IsValueEmpty(currentId))
            {
                currentId = 0;
            }
            else
            {
                currentId++;
            }

            await _identifierRepository.WriteAsync(0, currentId, true);

            await _templateRepository.WriteAsync(currentId, template, true);

            template.Id = currentId;

            _fileManager.CreateFilesForTemplate(currentId);
            _documentManagerStore.AddManager(currentId);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        if (!await _templateRepository.ExistsAsync(id, true))
        {
            return false;
        }

        var deleteAction = new Func<ValueTask>(async () =>
        {
            if (await _templateRepository.DeleteAsync(id, true))
            {
                _fileManager.DeleteFilesForTemplate(id);
                _documentManagerStore.DeleteManager(id);
            }
        });

        PendingActions.Enqueue(deleteAction);

        return true;
    }
}
