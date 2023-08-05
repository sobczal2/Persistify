using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Transactions;

public interface ITransactionManager
{
    ValueTask BeginAsync();
    ValueTask CommitAsync();
    ValueTask RollbackAsync();
}
