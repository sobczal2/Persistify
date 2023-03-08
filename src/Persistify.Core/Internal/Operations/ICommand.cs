using System.Threading.Tasks;

namespace Persistify.Core.Internal.Operations
{
    public interface ICommand<T> : IOperation<T>
    {
    }
}