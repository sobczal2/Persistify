using OneOf.Types;

namespace Persistify.Stores.Common;

public enum StoreErrorType
{
    Unknown,
    NotFound,
    AlreadyExists,
    StorageError,
    InvalidState,
}