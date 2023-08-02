using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;

namespace Persistify.Server.Management.Domain;

public static class ManagementDomainExtensions
{
    public static IServiceCollection AddManagementDomain(this IServiceCollection services)
    {

        return services;
    }
}
