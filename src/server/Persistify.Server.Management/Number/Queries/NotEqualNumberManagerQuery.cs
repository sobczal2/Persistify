using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Persistence.DataStructures.Abstractions;

namespace Persistify.Server.Management.Number.Queries;

public class NotEqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public NotEqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }

    // TODO: Refactor this to use some all documents lookup instead of two range lookups
    public override async ValueTask<List<NumberManagerHit>> Evaluate(IAsyncLookup<double, long> lookup)
    {
        var documentLeftIds = await lookup.GetRangeAsync(double.MinValue, Value - double.Epsilon);
        var documentRightIds = await lookup.GetRangeAsync(Value + double.Epsilon, double.MaxValue);
        var hits = new List<NumberManagerHit>(documentLeftIds.Count + documentRightIds.Count);
        foreach (var documentId in documentLeftIds)
        {
            hits.Add(new NumberManagerHit(documentId));
        }
        foreach (var documentId in documentRightIds)
        {
            hits.Add(new NumberManagerHit(documentId));
        }

        return hits;
    }
}
