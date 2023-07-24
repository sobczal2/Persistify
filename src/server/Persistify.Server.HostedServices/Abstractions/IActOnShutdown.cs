using System.Threading.Tasks;

namespace Persistify.Server.HostedServices.Abstractions;

public interface IActOnShutdown
{
    ValueTask PerformShutdownActionAsync();
}
