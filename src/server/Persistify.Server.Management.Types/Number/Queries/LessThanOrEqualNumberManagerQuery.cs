using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Persistence.DataStructures.Abstractions;

namespace Persistify.Server.Management.Types.Number.Queries;

public class LessThanOrEqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public LessThanOrEqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(
        templateFieldIdentifier)
    {
        Value = value;
    }

    public override async ValueTask<List<NumberManagerHit>> Evaluate(IAsyncLookup<double, long> lookup)
    {
        var documentIds = await lookup.GetRangeAsync(double.MinValue, Value);
        var hits = new List<NumberManagerHit>(documentIds.Count);
        foreach (var documentId in documentIds)
        {
            hits.Add(new NumberManagerHit(documentId));
        }

        return hits;
    }
}
