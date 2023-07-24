using System;
using System.Threading.Tasks;

namespace Persistify.Server.HostedServices.Abstractions;

public interface IActRecurrently
{
    TimeSpan RecurrentActionInterval { get; }
    ValueTask PerformRecurrentActionAsync();
}
