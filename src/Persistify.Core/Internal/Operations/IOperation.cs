using System.Threading.Tasks;

namespace Persistify.Core.Internal.Operations
{
    public interface IOperation<T>
    {
        Task ExecuteAsync();
    }
}