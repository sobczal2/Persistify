using System.Threading.Tasks;

namespace Persistify.Server.HostedServices.Abstractions;

public interface IActOnStartup
{
    ValueTask PerformStartupActionAsync();
}
