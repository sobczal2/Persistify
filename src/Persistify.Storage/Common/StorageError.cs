namespace Persistify.Storage.Common;

public struct StorageError
{
    public string Message { get; }

    public StorageError(string message)
    {
        Message = message;
    }
}