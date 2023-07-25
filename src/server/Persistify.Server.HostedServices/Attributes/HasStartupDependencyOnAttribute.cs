using System;

namespace Persistify.Server.HostedServices.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HasStartupDependencyOnAttribute : Attribute
{
    public Type[] DependencyTypes { get; }

    public HasStartupDependencyOnAttribute(params Type[] dependencyTypes)
    {
        DependencyTypes = dependencyTypes;
    }
}
