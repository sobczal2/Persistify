using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Persistify.Server.Errors;

public static class ExceptionExtensions
{
    public static void Throw(this Exception exception)
    {
#if DEBUG
        throw exception;
#else
        ExceptionDispatchInfo.Capture(exception).Throw();
#endif
    }
}
