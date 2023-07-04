using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.DataStructures.PrefixTree;
using Persistify.Helpers.Strings;
using Persistify.Management.Common;
using Persistify.Management.Fts.Search;
using Persistify.Management.Fts.Token;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Fts.Manager;

public class PrefixTreeFtsManager : IFtsManager
{
    private readonly ITokenizer _tokenizer;
    private readonly IScoreCalculator _defaultScoreCalculator;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, PrefixTree<PrefixTreeFtsValue>> _prefixTrees;

    public PrefixTreeFtsManager(ITokenizer tokenizer, IScoreCalculator defaultScoreCalculator)
    {
        _tokenizer = tokenizer;
        _defaultScoreCalculator = defaultScoreCalculator;
        _prefixTrees = new ConcurrentDictionary<TemplateFieldIdentifier, PrefixTree<PrefixTreeFtsValue>>();
    }

    public ValueTask AddAsync(string templateName, Document document, ulong documentId,
        CancellationToken cancellationToken = default)
    {
        foreach (var field in document.TextFields)
        {
            var prefixTree = _prefixTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, field.FieldName),
                _ => new PrefixTree<PrefixTreeFtsValue>());
            var tokens = _tokenizer.Tokenize(field.Value);
            for(var tokenIndex = 0; tokenIndex < tokens.Count; tokenIndex++)
            {
                var token = tokens.ElementAt(tokenIndex);
                var suffixes = StringHelpers.GetSuffixes(token);
                prefixTree.Add(suffixes[0], new PrefixTreeFtsValue(documentId, PrefixTreeValueFlags.Exact));
                for(var i = 1; i < suffixes.Length; i++)
                {
                    prefixTree.Add(suffixes[i], new PrefixTreeFtsValue(documentId, PrefixTreeValueFlags.Suffix));
                }
            }
        }
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<ICollection<FtsSearchHit>> SearchAsync(string templateName, FtsQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var foundIds = new Dictionary<ulong, int>();
        var prefixTree = _prefixTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, query.FieldName),
            _ => new PrefixTree<PrefixTreeFtsValue>());
        
        var tokens = _tokenizer.TokenizeWithWildcards(query.Value);
        foreach (var token in tokens)
        {
            var prefixTreeValues = prefixTree.Search(token, query.CaseSensitive, query.Exact);
            foreach (var prefixTreeValue in prefixTreeValues)
            {
                if(query.Exact && prefixTreeValue.Flags.HasFlag(PrefixTreeValueFlags.Suffix))
                    continue;
                if (foundIds.ContainsKey(prefixTreeValue.DocumentId))
                {
                    foundIds[prefixTreeValue.DocumentId]++;
                }
                else
                {
                    foundIds.Add(prefixTreeValue.DocumentId, 1);
                }
            }
        }

        var result = new FtsSearchHit[foundIds.Count];
        
        var index = 0;
        foreach (var foundId in foundIds)
        {
            result[index] = new FtsSearchHit(foundId.Key, scoreCalculator.Calculate(foundId.Value));
            index++;
        }
        
        return ValueTask.FromResult<ICollection<FtsSearchHit>>(result);
    }

    public ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default)
    {
        // ReSharper disable once ConvertToLocalFunction
        Predicate<PrefixTreeFtsValue> predicate = x => x.DocumentId == documentId;
        var prefixTrees = _prefixTrees.Where(x => x.Key.TemplateName == templateName).ToList();
        foreach (var prefixTree in prefixTrees)
        {
            prefixTree.Value.Remove(predicate);
        }
        
        return ValueTask.CompletedTask;
    }
}
