using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Persistify.DataStructures.IntervalTree;
using Persistify.Management.Common;
using Persistify.Management.Number.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Shared;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Number.Manager;

public class IntervalTreeNumberManager : INumberManager
{
    private readonly IScoreCalculator _defaultScoreCalculator;

    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<IntervalTreeNumberValue>>
        _intervalTrees;

    public IntervalTreeNumberManager(IScoreCalculator defaultScoreCalculator)
    {
        _defaultScoreCalculator = defaultScoreCalculator;
        _intervalTrees = new ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<IntervalTreeNumberValue>>();
    }

    public void Add(string templateName, Document document, long documentId)
    {
        foreach (var field in document.NumberFields)
        {
            var intervalTree = _intervalTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, field.FieldName),
                _ => new IntervalTree<IntervalTreeNumberValue>());
            intervalTree.Add(new IntervalTreeNumberValue(field.Value, documentId));
        }
    }

    public IEnumerable<NumberSearchHit> Search(string templateName, NumberQuery query,
        IScoreCalculator? scoreCalculator = null)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var foundIds = new Dictionary<long, int>();

        if (!_intervalTrees.TryGetValue(new TemplateFieldIdentifier(templateName, query.FieldName),
                out var intervalTree))
        {
            return Array.Empty<NumberSearchHit>();
        }

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

        return result;
    }

    public void Delete(string templateName, long documentId)
    {
        // ReSharper disable once ConvertToLocalFunction
        Predicate<IntervalTreeNumberValue> predicate = value => value.DocumentId == documentId;
        var intervalTrees = _intervalTrees.Where(pair => pair.Key.TemplateName == templateName).ToList();
        foreach (var intervalTree in intervalTrees)
        {
            intervalTree.Value.Remove(predicate);
        }
    }
}
