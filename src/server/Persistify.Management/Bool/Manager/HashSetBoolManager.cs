using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Persistify.Management.Bool.Search;
using Persistify.Management.Common;
using Persistify.Management.Score;

namespace Persistify.Management.Bool.Manager;

public class HashSetBoolManager : IBoolManager
{
    private readonly IScoreCalculator _defaultScoreCalculator;

    // HashSets are slow, reconsider using a different data structure
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>> _falseHashSets;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>> _trueHashSets;

    public HashSetBoolManager(IScoreCalculator defaultScoreCalculator)
    {
        _defaultScoreCalculator = defaultScoreCalculator;
        _trueHashSets = new ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>>();
        _falseHashSets = new ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>>();
    }

    public void Add(string templateName, Protos.Documents.Shared.Document document, long documentId)
    {
        foreach (var field in document.BoolFields)
        {
            var hashSet = GetOrAddHashSet(templateName, field.FieldName, field.Value);
            hashSet.Add(documentId);
        }
    }

    public IEnumerable<BoolSearchHit> Search(string templateName, BoolQuery query,
        IScoreCalculator? scoreCalculator = null)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var score = scoreCalculator.Calculate(1); // result of bool queries don't have duplicates

        var set = GetHashSet(templateName, query.FieldName, query.Value);

        return set?.Select(documentId => new BoolSearchHit(documentId, score)).ToList() ??
               Enumerable.Empty<BoolSearchHit>();
    }

    public void Delete(string templateName, long documentId)
    {
        foreach (var hashSet in _trueHashSets.Values)
        {
            hashSet.Remove(documentId);
        }

        foreach (var hashSet in _falseHashSets.Values)
        {
            hashSet.Remove(documentId);
        }

        DropEmptyHashSets();
    }

    private ISet<long> GetOrAddHashSet(string templateName, string fieldName, bool value)
    {
        return value
            ? _trueHashSets.GetOrAdd(new TemplateFieldIdentifier(templateName, fieldName), _ => new HashSet<long>())
            : _falseHashSets.GetOrAdd(new TemplateFieldIdentifier(templateName, fieldName), _ => new HashSet<long>());
    }

    private IEnumerable<long>? GetHashSet(string templateName, string fieldName, bool value)
    {
        return value
            ? _trueHashSets.TryGetValue(new TemplateFieldIdentifier(templateName, fieldName), out var hashSet)
                ? hashSet
                : null
            : _falseHashSets.TryGetValue(new TemplateFieldIdentifier(templateName, fieldName), out hashSet)
                ? hashSet
                : null;
    }

    private void DropEmptyHashSets()
    {
        foreach (var key in _trueHashSets.Keys.Where(key => _trueHashSets[key].Count == 0))
        {
            if (!_trueHashSets.TryRemove(key, out _))
            {
                throw new ManagerException("Failed to remove empty hash set. This may be a concurrency issue.");
            }
        }

        foreach (var key in _falseHashSets.Keys.Where(key => _falseHashSets[key].Count == 0))
        {
            if (!_falseHashSets.TryRemove(key, out _))
            {
                throw new ManagerException("Failed to remove empty hash set. This may be a concurrency issue.");
            }
        }
    }
}
