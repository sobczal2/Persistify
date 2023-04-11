namespace Persistify.Storage.Common;

public class StorageSuccess<T>
{
    public StorageSuccess(T data)
    {
        Data = data;
    }

    public T Data { get; }
}