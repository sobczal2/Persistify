using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Persistance.Template;

namespace Persistify.Management.Template;

public class DictionaryTemplateManager : ITemplateManager
{
    private readonly ConcurrentDictionary<string, Protos.Templates.Shared.Template> _templates;
    private readonly ITemplateStorage _templateStorage;

    public DictionaryTemplateManager(ITemplateStorage templateStorage)
    {
        _templateStorage = templateStorage;
        _templates = new ConcurrentDictionary<string, Protos.Templates.Shared.Template>();
    }

    public async ValueTask AddAsync(Protos.Templates.Shared.Template template)
    {
        _templates.TryAdd(template.Name, template);
        await _templateStorage.AddAsync(template);
    }

    public Protos.Templates.Shared.Template? Get(string templateName)
    {
        return _templates.TryGetValue(templateName, out var template) ? template : null;
    }

    public bool Exists(string templateName)
    {
        return _templates.ContainsKey(templateName);
    }

    public IEnumerable<Protos.Templates.Shared.Template> GetAll()
    {
        return _templates.Values;
    }

    public async ValueTask DeleteAsync(string templateName)
    {
        _templates.TryRemove(templateName, out _);
        await _templateStorage.DeleteAsync(templateName);
    }

    public async ValueTask LoadAsync()
    {
        var templates = await _templateStorage.GetAllAsync();
        foreach (var template in templates)
        {
            _templates.TryAdd(template.Name, template);
        }
    }
}
