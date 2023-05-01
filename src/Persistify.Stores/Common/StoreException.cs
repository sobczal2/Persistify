using System;

namespace Persistify.Stores.Common;

public class StoreException : Exception
{
    public StoreException(string message) : base(message)
    {
    }
}