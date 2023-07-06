using System.Collections.Concurrent;
using System.Reflection;
using Persistify.DataStructures.IntervalTree;
using Persistify.Management.Common;
using Persistify.Management.Number.Manager;
using Persistify.Management.Number.Search;

namespace Persistify.Management.Tests.Unit.Number;

public static class IntervalTreeNumberManagerTestExtensions
{
    public static ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<IntervalTreeNumberValue>>
        GetIntervalTrees(this INumberManager numberManager)
    {
        return (ConcurrentDictionary<TemplateFieldIdentifier, IIntervalTree<IntervalTreeNumberValue>>)
            typeof(IntervalTreeNumberManager)
                .GetField("_intervalTrees", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(numberManager)!;
    }

    public static IntervalTree<IntervalTreeNumberValue>? GetIntervalTree(this INumberManager numberManager,
        string templateName, string fieldName)
    {
        var intervalTrees = numberManager.GetIntervalTrees();
        var fieldIdentifier = new TemplateFieldIdentifier(templateName, fieldName);
        return intervalTrees.TryGetValue(fieldIdentifier, out var intervalTree)
            ? (IntervalTree<IntervalTreeNumberValue>?)intervalTree
            : null;
    }
}
