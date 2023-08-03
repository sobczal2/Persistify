using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Persistence.DataStructures.Abstractions;

namespace Persistify.Server.Management.Number.Queries;

public class LessThanNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public LessThanNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }

    public override async ValueTask<List<NumberManagerHit>> Evaluate(IAsyncLookup<double, int> lookup)
    {
        var documentIds = await lookup.GetRangeAsync(double.MinValue, Value - double.Epsilon);
        var hits = new List<NumberManagerHit>(documentIds.Count);
        foreach (var documentId in documentIds)
        {
            hits.Add(new NumberManagerHit(documentId));
        }

        return hits;
    }
}
