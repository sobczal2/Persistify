using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Persistence.DataStructures.Abstractions;

namespace Persistify.Server.Management.Number.Queries;

public class RangeNumberManagerQuery : NumberManagerQuery
{
    private readonly bool _isMinInclusive;
    private readonly bool _isMaxInclusive;
    public double MinValue { get; }
    public double MaxValue { get; }

    public RangeNumberManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, double minValue, double maxValue, bool isMinInclusive, bool isMaxInclusive) :
        base(templateFieldIdentifier)
    {
        _isMinInclusive = isMinInclusive;
        _isMaxInclusive = isMaxInclusive;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public override async ValueTask<List<NumberManagerHit>> Evaluate(IAsyncLookup<double, int> lookup)
    {
        var minValue = _isMinInclusive ? MinValue : MinValue + double.Epsilon;
        var maxValue = _isMaxInclusive ? MaxValue : MaxValue - double.Epsilon;
        var documentIds = await lookup.GetRangeAsync(minValue, maxValue);
        var hits = new List<NumberManagerHit>(documentIds.Count);
        foreach (var documentId in documentIds)
        {
            hits.Add(new NumberManagerHit(documentId));
        }

        return hits;
    }
}
