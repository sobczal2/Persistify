using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Concurrency;

public static class SemaphoreSlimExtensions
{
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

    public static TResult Wrap<TResult>(this SemaphoreSlim semaphore, Func<TResult> func)
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

    public static async ValueTask<TResult> WrapAsync<TResult>(
        this SemaphoreSlim semaphore,
        Func<ValueTask<TResult>> func
    )
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
}
