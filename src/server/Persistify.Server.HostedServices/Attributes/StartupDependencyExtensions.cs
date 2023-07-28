using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Persistify.Server.HostedServices.Attributes;

public static class StartupDependencyExtensions
{
    public static int GetStartupPriority(this Type type)
    {
        var attribute = type.GetCustomAttribute<StartupPriority>();
        if (attribute is null)
        {
            return 0;
        }

        return attribute.Priority;
    }
}
