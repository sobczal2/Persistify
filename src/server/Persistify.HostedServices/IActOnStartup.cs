using System.Threading.Tasks;

namespace Persistify.HostedServices;

public interface IActOnStartup
{
    ValueTask PerformStartupActionAsync();
}
