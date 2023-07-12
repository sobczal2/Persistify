using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Management.Common;
using Persistify.Persistance.Template;

namespace Persistify.Management.Template;

public class DictionaryTemplateManager : ITemplateManager
{
    private readonly ConcurrentDictionary<string, long> _templateIds;
    private readonly ConcurrentDictionary<string, Protos.Templates.Shared.Template> _templates;
    private readonly ITemplateStorage _templateStorage;

    public DictionaryTemplateManager(ITemplateStorage templateStorage)
    {
        _templateStorage = templateStorage;
        _templates = new ConcurrentDictionary<string, Protos.Templates.Shared.Template>();
        _templateIds = new ConcurrentDictionary<string, long>();
    }

    public long GetNextId(string templateName)
    {
        if (!_templateIds.ContainsKey(templateName))
            throw new ManagerException($"Template {templateName} does not exist");

        return _templateIds.AddOrUpdate(templateName, 1, (_, value) => value + 1);
    }

    public async ValueTask AddAsync(Protos.Templates.Shared.Template template)
    {
        if (!_templates.TryAdd(template.Name, template))
            throw new ManagerException($"Template {template.Name} already exists");

        if (!_templateIds.TryAdd(template.Name, 0))
            throw new ManagerException($"Template {template.Name} already exists");

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
        if (!_templates.TryRemove(templateName, out _))
            throw new ManagerException($"Template {templateName} does not exist");

        if (!_templateIds.TryRemove(templateName, out _))
            throw new ManagerException($"Template {templateName} does not exist");

        await _templateStorage.DeleteAsync(templateName);
    }

    public async ValueTask LoadAsync()
    {
        var templates = (await _templateStorage.GetAllAsync()).ToList();
        var templateIds = await _templateStorage.GetTemplateIdsAsync();
        if (templates.Count != templateIds.Count)
            throw new ManagerException("Data corrupted: Template count and template id count do not match");

        foreach (var template in templates)
            _templates.TryAdd(template.Name, template);

        foreach (var (key, value) in templateIds)
            _templateIds.TryAdd(key, value);
    }

    public async ValueTask SaveAsync()
    {
        await _templateStorage.SaveTemplateIdsAsync(_templateIds);
    }
}
