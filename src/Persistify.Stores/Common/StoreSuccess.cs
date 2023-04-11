namespace Persistify.Stores.Common;

public struct StoreSuccess<T>
{
    public T Data { get; }

    public StoreSuccess(T data)
    {
        Data = data;
    }
}