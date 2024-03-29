﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Abstractions;

public interface IValueTypeStreamRepository<TValue>
{
    TValue EmptyValue { get; }

    ValueTask<TValue> ReadAsync(
        int key,
        bool useLock
    );

    IAsyncEnumerable<(int key, TValue value)> ReadRangeAsync(
        int take,
        int skip,
        bool useLock
    );

    ValueTask<int> CountAsync(
        bool useLock
    );

    ValueTask WriteAsync(
        int key,
        TValue value,
        bool useLock
    );

    ValueTask<bool> DeleteAsync(
        int key,
        bool useLock
    );

    void Clear(
        bool useLock
    );

    bool IsValueEmpty(
        TValue value
    );
}
