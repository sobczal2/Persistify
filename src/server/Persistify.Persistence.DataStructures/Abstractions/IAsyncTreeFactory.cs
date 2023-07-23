namespace Persistify.Persistence.DataStructures.Abstractions;

public interface IAsyncTreeFactory
{
    IAsyncTree<T> Create<T>(string name, IComparer<T> comparer) where T : notnull;
}
