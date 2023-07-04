using System.Collections.Concurrent;
using System.Reflection;
using Persistify.DataStructures.PrefixTree;
using Persistify.Management.Common;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Search;

namespace Persistify.Management.Test.Fts;

public static class PrefixTreeFtsManagerTestExtensions
{
    public static ConcurrentDictionary<TemplateFieldIdentifier, PrefixTree<PrefixTreeFtsValue>> GetPrefixTrees(this IFtsManager ftsManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, PrefixTree<PrefixTreeFtsValue>>)typeof(PrefixTreeFtsManager)
            .GetField("_prefixTrees", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(ftsManager)!;
    }
    
    public static PrefixTree<PrefixTreeFtsValue>? GetPrefixTree(this IFtsManager ftsManager, string templateName, string fieldName)
    {
        var prefixTrees = ftsManager.GetPrefixTrees();
        var fieldIdentifier = new TemplateFieldIdentifier(templateName, fieldName);
        return prefixTrees.TryGetValue(fieldIdentifier, out var prefixTree) ? prefixTree : null;
    }
}
