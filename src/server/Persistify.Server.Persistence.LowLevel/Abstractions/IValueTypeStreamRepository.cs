﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.LowLevel.Abstractions;

public interface IValueTypeStreamRepository<TValue>
{
    ValueTask<TValue> ReadAsync(int key);
    ValueTask<Dictionary<int, TValue>> ReadAllAsync();
    ValueTask WriteAsync(int key, TValue value);
    ValueTask<bool> DeleteAsync(int key);
    void Clear();
    bool IsValueEmpty(TValue value);
    TValue EmptyValue { get; }
}
