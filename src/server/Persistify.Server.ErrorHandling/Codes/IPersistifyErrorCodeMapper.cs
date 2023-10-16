using System;

namespace Persistify.Server.ErrorHandling.Codes;

public interface IPersistifyErrorCodeMapper<out TCode>
    where TCode : Enum
{
    TCode Map(PersistifyErrorCode errorCode);
}
