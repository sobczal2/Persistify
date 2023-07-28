using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncRangeTree<T> : IAsyncTree<T>
    where T : IComparable<double>
{
    ValueTask<List<T>> GetRangeAsync(double from, double to);
}
