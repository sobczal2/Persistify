using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface ITransactionManager
{
    ValueTask BeginAsync(IEnumerable<int> templateIds, bool write, bool global);
    ValueTask CommitAsync();
    ValueTask RollbackAsync();
}
