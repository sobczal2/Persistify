using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Transactions;

public interface ITransactionManager
{
    ValueTask BeginAsync(IEnumerable<int> templateIds, bool write, bool global);
    ValueTask CommitAsync();
    ValueTask RollbackAsync();
}
