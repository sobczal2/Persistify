using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Persistify.DataStructures.PrefixTree;
using Persistify.Helpers.Strings;
using Persistify.Management.Common;
using Persistify.Management.Fts.Search;
using Persistify.Management.Fts.Token;
using Persistify.Management.Score;
using Persistify.Protos.Documents.Shared;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Fts.Manager;

public class PrefixTreeFtsManager : IFtsManager
{
    private readonly IScoreCalculator _defaultScoreCalculator;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IPrefixTree<PrefixTreeFtsValue>> _prefixTrees;
    private readonly ITokenizer _tokenizer;

    public PrefixTreeFtsManager(ITokenizer tokenizer, IScoreCalculator defaultScoreCalculator)
    {
        _tokenizer = tokenizer;
        _defaultScoreCalculator = defaultScoreCalculator;
        _prefixTrees = new ConcurrentDictionary<TemplateFieldIdentifier, IPrefixTree<PrefixTreeFtsValue>>();
    }

    public void Add(string templateName, Document document, long documentId)
    {
        foreach (var field in document.TextFields)
        {
            var prefixTree = _prefixTrees.GetOrAdd(new TemplateFieldIdentifier(templateName, field.FieldName),
                _ => new PrefixTree<PrefixTreeFtsValue>());
            var tokens = _tokenizer.Tokenize(field.Value);
            foreach (var token in tokens)
            {
                var suffixes = StringHelpers.GetSuffixes(token.Term);

                prefixTree.Add(suffixes[0],
                    new PrefixTreeFtsValue(documentId, token.TermFrequency, PrefixTreeValueFlags.Exact));

                for (var i = 1; i < suffixes.Length; i++)
                {
                    prefixTree.Add(suffixes[i],
                        new PrefixTreeFtsValue(documentId, token.TermFrequency, PrefixTreeValueFlags.Suffix));
                }
            }
        }
    }

    public IEnumerable<FtsSearchHit> Search(string templateName, FtsQuery query,
        IScoreCalculator? scoreCalculator = null)
    {
        scoreCalculator ??= _defaultScoreCalculator;
        var foundIds = new Dictionary<long, float>();

        if (!_prefixTrees.TryGetValue(new TemplateFieldIdentifier(templateName, query.FieldName), out var prefixTree))
        {
            return Array.Empty<FtsSearchHit>();
        }

        var tokens = _tokenizer.TokenizeWithWildcards(query.Value);
        foreach (var token in tokens)
        {
            var prefixTreeValues = prefixTree.Search(token, query.CaseSensitive, query.Exact);
            foreach (var prefixTreeValue in prefixTreeValues)
            {
                if (query.Exact && prefixTreeValue.Flags.HasFlag(PrefixTreeValueFlags.Suffix))
                {
                    continue;
                }

                if (foundIds.TryGetValue(prefixTreeValue.DocumentId, out var score))
                {
                    foundIds[prefixTreeValue.DocumentId] = score + prefixTreeValue.TermFrequency;
                }
                else
                {
                    foundIds.Add(prefixTreeValue.DocumentId, prefixTreeValue.TermFrequency);
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

        return result;
    }

    public void Delete(string templateName, long documentId)
    {
        // ReSharper disable once ConvertToLocalFunction
        Predicate<PrefixTreeFtsValue> predicate = x => x.DocumentId == documentId;
        var prefixTrees = _prefixTrees.Where(x => x.Key.TemplateName == templateName).ToList();
        foreach (var prefixTree in prefixTrees)
        {
            prefixTree.Value.Remove(predicate);
        }
    }
}
