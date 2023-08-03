using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Persistence.DataStructures.Abstractions;

namespace Persistify.Server.Management.Number.Queries;

public abstract class NumberManagerQuery : ITypeManagerQuery
{
    protected NumberManagerQuery(TemplateFieldIdentifier templateFieldIdentifier)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
    }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }

    public abstract ValueTask<List<NumberManagerHit>> Evaluate(IAsyncLookup<double, int> lookup);
}
