using System.Collections.Generic;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncTreeFactory
{
    IAsyncTree<T> Create<T>(string name, IComparer<T> comparer) where T : notnull;
}
