using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Common;

namespace Persistify.Management.Tests.Unit.Bool;

public static class HashSetBoolManagerTestExtensions
{
    public static ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>> GetTrueHashSets(
        this IBoolManager boolManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>>)typeof(HashSetBoolManager)
            .GetField("_trueHashSets", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(boolManager)!;
    }

    public static ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>> GetFalseHashSets(
        this IBoolManager boolManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, HashSet<long>>)typeof(HashSetBoolManager)
            .GetField("_falseHashSets", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(boolManager)!;
    }

    public static HashSet<long>? GetHashSet(this IBoolManager boolManager, string templateName, string fieldName,
        bool value)
    {
        var hashSets = value ? boolManager.GetTrueHashSets() : boolManager.GetFalseHashSets();
        var fieldIdentifier = new TemplateFieldIdentifier(templateName, fieldName);
        return hashSets.TryGetValue(fieldIdentifier, out var hashSet) ? hashSet : null;
    }
}
