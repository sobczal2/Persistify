using System;

namespace Persistify.Management.Common;

public class ManagerErrorException : Exception
{
    public ManagerErrorException(string message) : base(message)
    {
    }
}
