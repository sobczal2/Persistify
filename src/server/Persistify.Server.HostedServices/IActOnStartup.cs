using System.Threading.Tasks;

namespace Persistify.Server.HostedServices;

public interface IActOnStartup
{
    ValueTask PerformStartupActionAsync();
}
