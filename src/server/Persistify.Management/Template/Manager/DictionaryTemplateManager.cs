using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Management.Template.Cache;
using Persistify.Persistance.Document;
using Persistify.Persistance.Template;

namespace Persistify.Management.Template.Manager;

public class DictionaryTemplateManager : ITemplateManager
{
    private readonly IDocumentStorage _documentStorage;
    private readonly ITemplateCache _templateCache;
    private readonly ITemplateStorage _templateStorage;

    public DictionaryTemplateManager(
        ITemplateStorage templateStorage,
        ITemplateCache templateCache,
        IDocumentStorage documentStorage
    )
    {
        _templateStorage = templateStorage;
        _templateCache = templateCache;
        _documentStorage = documentStorage;
    }

    public async ValueTask AddAsync(Protos.Templates.Shared.Template template)
    {
        await _templateStorage.AddAsync(template);
        await _documentStorage.AddSpaceForTemplateAsync(template.Name);
    }

    public async ValueTask<Protos.Templates.Shared.Template?> GetAsync(string templateName)
    {
        var template = _templateCache.Get(templateName);
        if (template != null)
        {
            return template;
        }

        template = await _templateStorage.GetAsync(templateName);
        if (template != null)
        {
            _templateCache.Set(templateName, template);
        }

        return template;
    }

    public ValueTask<bool> ExistsAsync(string templateName)
    {
        var template = _templateCache.Get(templateName);
        if (template != null)
        {
            return ValueTask.FromResult(true);
        }

        return _templateStorage.ExistsAsync(templateName);
    }

    public async ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync()
    {
        return await _templateStorage.GetAllAsync();
    }

    public async ValueTask DeleteAsync(string templateName)
    {
        _templateCache.Remove(templateName);
        await _templateStorage.DeleteAsync(templateName);
        await _documentStorage.DeleteSpaceForTemplateAsync(templateName);
    }
}
