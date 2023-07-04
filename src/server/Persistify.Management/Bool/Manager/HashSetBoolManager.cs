using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Management.Bool.Search;
using Persistify.Management.Common;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Bool.Manager;

public class HashSetBoolManager : IBoolManager
{
    private readonly IScoreCalculator _defaultScoreCalculator;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>> _trueHashSets;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>> _falseHashSets;
    
    public HashSetBoolManager(IScoreCalculator defaultScoreCalculator)
    {
        _defaultScoreCalculator = defaultScoreCalculator;
        _trueHashSets = new ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>>();
        _falseHashSets = new ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>>();
    }
    
    public ValueTask AddAsync(string templateName, Document document, ulong documentId,
        CancellationToken cancellationToken = default)
    {
        foreach (var field in document.BoolFields)
        {
            var hashSet = GetHashSet(templateName, field.FieldName, field.Value);
            hashSet.Add(documentId);
        }
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<ICollection<BoolSearchHit>> SearchAsync(string templateName, BoolQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var score = scoreCalculator.Calculate(1); // result of bool queries don't have duplicates
        var result = new List<BoolSearchHit>();
        
        var set = GetHashSet(templateName, query.FieldName, query.Value);
        foreach (var documentId in set)
        {
            result.Add(new BoolSearchHit(documentId, score));
        }
        
        return new ValueTask<ICollection<BoolSearchHit>>(result);
    }

    public ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default)
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
        
        return ValueTask.CompletedTask;
    }
    
    private ISet<ulong> GetHashSet(string templateName, string fieldName, bool value)
    {
        return value
            ? _trueHashSets.GetOrAdd(new TemplateFieldIdentifier(templateName, fieldName), _ => new HashSet<ulong>())
            : _falseHashSets.GetOrAdd(new TemplateFieldIdentifier(templateName, fieldName), _ => new HashSet<ulong>());
    }
    
    private void DropEmptyHashSets()
    {
        foreach (var key in _trueHashSets.Keys.Where(key => _trueHashSets[key].Count == 0))
        {
            if (!_trueHashSets.TryRemove(key, out _))
            {
                throw new ManagerErrorException("Failed to remove empty hash set. This may be a concurrency issue.");
            }
        }
        
        foreach (var key in _falseHashSets.Keys.Where(key => _falseHashSets[key].Count == 0))
        {
            if(!_falseHashSets.TryRemove(key, out _))
            {
                throw new ManagerErrorException("Failed to remove empty hash set. This may be a concurrency issue.");
            }
        }
    }
}
