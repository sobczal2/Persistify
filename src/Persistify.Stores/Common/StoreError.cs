using OneOf.Types;

namespace Persistify.Stores.Common;

public struct StoreError
{
    public string Message { get; }
    public StoreErrorType Type { get; }
    
    public StoreError(string message, StoreErrorType type)
    {
        Message = message;
        Type = type;
    }
}