using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Repositories;

public interface IRepository
{
    ValueTask BeginReadAsync(long transactionId);
    ValueTask BeginWriteAsync(long transactionId);

    ValueTask EndReadAsync(long transactionId);
    ValueTask EndWriteAsync(long transactionId);

    ValueTask FlushAsync();
}
