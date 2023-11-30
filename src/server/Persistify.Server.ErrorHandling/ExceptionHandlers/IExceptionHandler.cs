using System;

namespace Persistify.Server.ErrorHandling.ExceptionHandlers;

public interface IExceptionHandler
{
    void Handle(
        Exception exception
    );
}
