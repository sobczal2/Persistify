using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Tests.Unit;

public static class DocumentIdManagerTestExtensions
{
    public static ISet<int> GetInitializedTemplates(this DocumentIdManager documentIdManager)
    {
        var field = typeof(DocumentIdManager).GetField("_initializedTemplates", BindingFlags.NonPublic | BindingFlags.Instance);
        return (ISet<int>)field!.GetValue(documentIdManager)!;
    }

    public static SemaphoreSlim GetSemaphoreSlim(this DocumentIdManager documentIdManager)
    {
        var field = typeof(DocumentIdManager).GetField("_semaphoreSlim", BindingFlags.NonPublic | BindingFlags.Instance);
        return (SemaphoreSlim)field!.GetValue(documentIdManager)!;
    }

    public static IIntLinearRepositoryManager GetLinearRepositoryManager(this DocumentIdManager documentIdManager)
    {
        var field = typeof(DocumentIdManager).GetField("_linearRepositoryManager", BindingFlags.NonPublic | BindingFlags.Instance);
        return (IIntLinearRepositoryManager)field!.GetValue(documentIdManager)!;
    }

    public static void SetInitializedTemplates(this DocumentIdManager documentIdManager, ISet<int> initializedTemplates)
    {
        var field = typeof(DocumentIdManager).GetField("_initializedTemplates", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(documentIdManager, initializedTemplates);
    }

    public static void SetSemaphoreSlim(this DocumentIdManager documentIdManager, SemaphoreSlim semaphoreSlim)
    {
        var field = typeof(DocumentIdManager).GetField("_semaphoreSlim", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(documentIdManager, semaphoreSlim);
    }

    public static void SetLinearRepositoryManager(this DocumentIdManager documentIdManager, IIntLinearRepositoryManager intLinearRepositoryManager)
    {
        var field = typeof(DocumentIdManager).GetField("_linearRepositoryManager", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(documentIdManager, intLinearRepositoryManager);
    }

    public static void SetLinearRepository(this DocumentIdManager documentIdManager, IIntLinearRepository intLinearRepository)
    {
        var field = typeof(DocumentIdManager).GetField("_linearRepository", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(documentIdManager, intLinearRepository);
    }
}
