using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Persistify.Server.HostedServices.Attributes;

public static class StartupDependencyExtensions
{
    public static IEnumerable<Type> GetStartupDependencies(this Type type)
    {
        var attribute = type.GetCustomAttribute<HasStartupDependencyOnAttribute>();
        if (attribute is null)
        {
            return Enumerable.Empty<Type>();
        }

        return attribute.DependencyTypes;
    }
}
