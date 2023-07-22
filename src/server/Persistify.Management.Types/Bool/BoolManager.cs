using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Bool;

public class BoolManager : ITypeManager<BoolManagerQuery, BoolManagerHit>
{
    public ValueTask<IEnumerable<BoolManagerHit>> SearchAsync(BoolManagerQuery query)
    {
        return ValueTask.FromResult<IEnumerable<BoolManagerHit>>(new List<BoolManagerHit>());
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
