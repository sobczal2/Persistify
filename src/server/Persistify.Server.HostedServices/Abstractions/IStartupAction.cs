using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Server.HostedServices.Abstractions;

public interface IStartupAction
{
    public string Name { get; }
    public ValueTask PerformStartupActionAsync(CancellationToken cancellationToken);
}
