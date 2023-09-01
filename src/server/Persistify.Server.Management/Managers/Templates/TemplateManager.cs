using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Transactions.Exceptions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Templates;

public class TemplateManager : Manager, ITemplateManager
{
    private readonly IFileManager _fileManager;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<Template> _innerTemplateRepository;

    private static readonly TemplateManagerRequiredFileGroup TemplateManagerRequiredFileGroup =
        new();

    public TemplateManager(
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IFileManager fileManager
    )
    {
        _fileManager = fileManager;
        var identifierFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.IdentifierFileName);
        var innerTemplateMainFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.InnerTemplateMainFileName);
        var innerTemplateOffsetLengthFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.InnerTemplateOffsetLengthFileName);

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

        var addAction = new Func<ValueTask>(async () =>
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

        PendingActions.Enqueue(addAction);
    }

    public void Remove(int id)
    {
        if (!CanWrite())
        {
            throw new NotAllowedForTransactionException();
        }

        var deleteAction = new Func<ValueTask>(async () =>
        {
            if (await _innerTemplateRepository.DeleteAsync(id))
            {
                _fileManager.DeleteFilesForTemplateAsync(id);
            }
        });

        PendingActions.Enqueue(deleteAction);
    }
}
