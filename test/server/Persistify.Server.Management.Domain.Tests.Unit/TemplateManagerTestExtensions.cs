using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Persistify.Domain.Templates;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Tests.Unit;

public static class TemplateManagerTestExtensions
{
    public static string GetTemplateRepositoryKey(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("TemplateRepositoryKey", BindingFlags.NonPublic | BindingFlags.Static);
        return (string)field!.GetValue(templateManager)!;
    }

    public static string GetDocumentRepositoryPrefix(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("DocumentRepositoryPrefix", BindingFlags.NonPublic | BindingFlags.Static);
        return (string)field!.GetValue(templateManager)!;
    }

    public static SemaphoreSlim GetGeneralTemplateSemaphore(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("_generalTemplateSemaphore", BindingFlags.NonPublic | BindingFlags.Instance);
        return (SemaphoreSlim)field!.GetValue(templateManager)!;
    }


    public static ConcurrentDictionary<int, SemaphoreSlim> GetIndividualSemaphores(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("_individualSemaphores", BindingFlags.NonPublic | BindingFlags.Instance);
        return (ConcurrentDictionary<int, SemaphoreSlim>)field!.GetValue(templateManager)!;
    }

    public static ConcurrentDictionary<string, int> GetTemplateNameToIdMap(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("_templateNameToIdMap", BindingFlags.NonPublic | BindingFlags.Instance);
        return (ConcurrentDictionary<string, int>)field!.GetValue(templateManager)!;
    }

    public static ConcurrentDictionary<int, Template> GetTemplates(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("_templates", BindingFlags.NonPublic | BindingFlags.Instance);
        return (ConcurrentDictionary<int, Template>)field!.GetValue(templateManager)!;
    }

    public static int GetLastTemplateId(this TemplateManager templateManager)
    {
        var field = typeof(TemplateManager).GetField("_lastTemplateId", BindingFlags.NonPublic | BindingFlags.Instance);
        return (int)field!.GetValue(templateManager)!;
    }

    public static int SetLastTemplateId(this TemplateManager templateManager, int value)
    {
        var field = typeof(TemplateManager).GetField("_lastTemplateId", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(templateManager, value);
        return value;
    }
}
