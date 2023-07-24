using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Number;

public class NumberManager : ITypeManager<NumberManagerQuery, NumberManagerHit>
{
    public ValueTask<IEnumerable<NumberManagerHit>> SearchAsync(NumberManagerQuery query)
    {
        return ValueTask.FromResult<IEnumerable<NumberManagerHit>>(new List<NumberManagerHit>());
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
