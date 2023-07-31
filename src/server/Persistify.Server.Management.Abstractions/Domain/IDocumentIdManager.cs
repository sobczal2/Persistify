using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface IDocumentIdManager
{
    ValueTask<long> GetNextIdAsync(int templateId);
    ValueTask<long> GetCurrentIdAsync(int templateId);
    ValueTask InitializeForTemplateAsync(int templateId);
    ValueTask RemoveForTemplateAsync(int templateId);
    ValueTask<IEnumerable<int>> GetInitializedTemplatesAsync();
    ValueTask<bool> ExistsForTemplateAsync(int templateId);
}
