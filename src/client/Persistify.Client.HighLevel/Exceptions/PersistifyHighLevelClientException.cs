using System;

namespace Persistify.Client.HighLevel.Exceptions;

public class PersistifyHighLevelClientException : Exception
{
    public PersistifyHighLevelClientException(
        string message
    )
        : base(message)
    {
    }
}
