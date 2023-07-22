using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Fts;

public class FtsManager : ITypeManager<ITypeManagerQuery, ITypeManagerHit>
{
    public ValueTask<IEnumerable<ITypeManagerHit>> SearchAsync(ITypeManagerQuery query)
    {
        return ValueTask.FromResult<IEnumerable<ITypeManagerHit>>(new List<ITypeManagerHit>());
    }

    public ValueTask IndexAsync(Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(long documentId)
    {
        return ValueTask.CompletedTask;
    }
}
