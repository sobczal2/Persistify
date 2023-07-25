using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Fts;

public class FtsManager : ITypeManager<FtsManagerQuery, FtsManagerHit>
{
    public ValueTask<IEnumerable<FtsManagerHit>> SearchAsync(int templateId, FtsManagerQuery query)
    {
        return new ValueTask<IEnumerable<FtsManagerHit>>(new List<FtsManagerHit>(0));
    }

    public ValueTask IndexAsync(int templateId, Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(int templateId, long documentId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask InitializeForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }
}
