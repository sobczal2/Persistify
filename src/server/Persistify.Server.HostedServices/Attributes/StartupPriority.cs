using System;

namespace Persistify.Server.HostedServices.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StartupPriority : Attribute
{
    public int Priority { get; }

    public StartupPriority(int priority)
    {
        Priority = priority;
    }
}
