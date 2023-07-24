using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Management.Domain.Abstractions;

public interface IDocumentIdManager
{
    ValueTask<long> GetNextId(int templateId);
    ValueTask<long> GetCurrentId(int templateId);
    ValueTask InitializeForTemplate(int templateId);
    ValueTask RemoveForTemplate(int templateId);
    ValueTask<IEnumerable<int>> GetInitializedTemplates();
}
