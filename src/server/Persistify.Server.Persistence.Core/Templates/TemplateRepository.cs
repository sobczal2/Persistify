using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Files;
using Persistify.Server.Persistence.Core.Repositories;
using Persistify.Server.Persistence.Core.Transactions.Exceptions;
using Persistify.Server.Persistence.LowLevel.Object;
using Persistify.Server.Persistence.LowLevel.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.Templates;

public class TemplateRepository : Repository, ITemplateRepository
{
    private readonly IFileManager _fileManager;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<Template> _innerTemplateRepository;

    public TemplateRepository(
        TemplateRepositoryRequiredFileDescriptor templateRepositoryRequiredFileDescriptor,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IFileManager fileManager
    )
    {
        _fileManager = fileManager;
        var identifierFileStream =
            fileStreamFactory.CreateStream(templateRepositoryRequiredFileDescriptor.IdentifierFileName);
        var innerTemplateMainFileStream =
            fileStreamFactory.CreateStream(templateRepositoryRequiredFileDescriptor.InnerTemplateMainFileName);
        var innerTemplateOffsetLengthFileStream =
            fileStreamFactory.CreateStream(templateRepositoryRequiredFileDescriptor.InnerTemplateOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _innerTemplateRepository = new ObjectStreamRepository<Template>(
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

        return await _innerTemplateRepository.ReadAsync(id);
    }

    public void Add(Template template)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        var addAction = new Func<Task>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0);
            if (_identifierRepository.IsValueEmpty(currentId))
            {
                currentId = 0;
            }
            else
            {
                currentId++;
            }

            await _identifierRepository.WriteAsync(0, currentId);

            await _innerTemplateRepository.WriteAsync(currentId, template);

            template.Id = currentId;

            _fileManager.CreateFilesForTemplateAsync(currentId);
        });

        CommitQueue.Enqueue(addAction);
    }

    public void Remove(int id)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        var deleteAction = new Func<Task>(async () =>
        {
            if (await _innerTemplateRepository.DeleteAsync(id))
            {
                _fileManager.DeleteFilesForTemplateAsync(id);
            }
        });

        CommitQueue.Enqueue(deleteAction);
    }
}
