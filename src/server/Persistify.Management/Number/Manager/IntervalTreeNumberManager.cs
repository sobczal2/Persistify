using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.DataStructures.IntervalTree;
using Persistify.Management.Common;
using Persistify.Management.Number.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Number.Manager;

public class IntervalTreeNumberManager : INumberManager
{
    private readonly IScoreCalculator _defaultScoreCalculator;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IntervalTree<IntervalTreeNumberValue>> _intervalTrees;

    public IntervalTreeNumberManager(IScoreCalculator defaultScoreCalculator)
    {
        _defaultScoreCalculator = defaultScoreCalculator;
        _intervalTrees = new ConcurrentDictionary<TemplateFieldIdentifier, IntervalTree<IntervalTreeNumberValue>>();
    }
    
    public ValueTask AddAsync(string templateName, Document document, ulong documentId,
        CancellationToken cancellationToken = default)
    {
        foreach (var field in document.NumberFields)
        {
            var intervalTree = _intervalTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, field.FieldName),
                _ => new IntervalTree<IntervalTreeNumberValue>());
            intervalTree.Add(new IntervalTreeNumberValue(field.Value, documentId));
        }
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<ICollection<NumberSearchHit>> SearchAsync(string templateName, NumberQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var foundIds = new Dictionary<ulong, int>();
        var intervalTree = _intervalTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, query.FieldName),
            _ => new IntervalTree<IntervalTreeNumberValue>());
        
        var hits = intervalTree.Search(query.MinValue, query.MaxValue);
        foreach (var hit in hits)
        {
            if (foundIds.ContainsKey(hit.DocumentId))
            {
                foundIds[hit.DocumentId]++;
            }
            else
            {
                foundIds.Add(hit.DocumentId, 1);
            }
        }
        
        var result = new NumberSearchHit[foundIds.Count];
        
        var index = 0;
        foreach (var foundId in foundIds)
        {
            result[index] = new NumberSearchHit(foundId.Key, scoreCalculator.Calculate(foundId.Value));
            index++;
        }
        
        return ValueTask.FromResult<ICollection<NumberSearchHit>>(result);
    }

    public ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default)
    {
        // ReSharper disable once ConvertToLocalFunction
        Predicate<IntervalTreeNumberValue> predicate = value => value.DocumentId == documentId;
        var intervalTrees = _intervalTrees.Where(pair => pair.Key.TemplateName == templateName).ToList();
        foreach (var intervalTree in intervalTrees)
        {
            intervalTree.Value.Remove(predicate);
        }

        return ValueTask.CompletedTask;
    }
}
