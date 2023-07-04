using System.Collections.Concurrent;
using System.Reflection;
using Persistify.DataStructures.IntervalTree;
using Persistify.Management.Common;
using Persistify.Management.Number.Manager;

namespace Persistify.Management.Test.Number;

public static class IntervalTreeNumberManagerTestExtensions
{
    public static ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<double>> GetIntervalTrees(this INumberManager numberManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<double>>)typeof(IntervalTreeNumberManager)
            .GetField("_intervalTrees", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(numberManager)!;
    }
    
    public static IIntervalTree<double>? GetIntervalTree(this INumberManager numberManager, string templateName, string fieldName)
    {
        var intervalTrees = numberManager.GetIntervalTrees();
        var fieldIdentifier = new TemplateFieldIdentifier(templateName, fieldName);
        return intervalTrees.TryGetValue(fieldIdentifier, out var intervalTree) ? intervalTree : null;
    }
}
