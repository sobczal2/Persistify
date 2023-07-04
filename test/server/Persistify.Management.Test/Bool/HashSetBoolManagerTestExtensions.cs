using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Common;

namespace Persistify.Management.Test.Bool;

public static class HashSetBoolManagerTestExtensions
{
    public static ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>> GetTrueHashSets(this IBoolManager boolManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>>)typeof(HashSetBoolManager)
            .GetField("_trueHashSets", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(boolManager)!;
    }
    
    public static ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>> GetFalseHashSets(this IBoolManager boolManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, HashSet<ulong>>)typeof(HashSetBoolManager)
            .GetField("_falseHashSets", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(boolManager)!;
    }
    
    public static HashSet<ulong>? GetHashSet(this IBoolManager boolManager, string templateName, string fieldName, bool value)
    {
        var hashSets = value ? boolManager.GetTrueHashSets() : boolManager.GetFalseHashSets();
        var fieldIdentifier = new TemplateFieldIdentifier(templateName, fieldName);
        return hashSets.TryGetValue(fieldIdentifier, out var hashSet) ? hashSet : null;
    }
}
