using System;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncTree<T>
{
    ValueTask InsertAsync(T value);
    ValueTask InsertOrPerformActionOnValueAsync<TArgs>(T value, Action<T, TArgs> action, TArgs args);
    ValueTask<T?> GetAsync(T value);
    ValueTask RemoveAsync(T value);
    ValueTask PerformActionByPredicateAndMaybeRemoveAsync<TArgs>(Func<T, TArgs, bool> predicate, Func<T, TArgs, bool> maybeRemoveAction, TArgs args);
}
