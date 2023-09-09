using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Helpers.Locking;

public static class SemaphoreHelpers
{
    public static async ValueTask WrapAsync(this SemaphoreSlim semaphore, Func<ValueTask> func)
    {
        await semaphore.WaitAsync();
        try
        {
            await func();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static async ValueTask<T> WrapAsync<T>(this SemaphoreSlim semaphore, Func<ValueTask<T>> func)
    {
        await semaphore.WaitAsync();
        try
        {
            return await func();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static T Wrap<T>(this SemaphoreSlim semaphore, Func<T> func)
    {
        semaphore.Wait();
        try
        {
            return func();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static void Wrap(this SemaphoreSlim semaphore, Action action)
    {
        semaphore.Wait();
        try
        {
            action();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
