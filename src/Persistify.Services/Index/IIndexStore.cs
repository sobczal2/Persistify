using System.Threading.Tasks;

namespace Persistify.Indexer.Index;

public interface IIndexStore
{
    Task<long> AddAsync();
}