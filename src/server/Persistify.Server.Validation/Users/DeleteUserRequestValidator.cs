using System;
using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Users;

public class DeleteUserRequestValidator : Validator<DeleteUserRequest>
{
    public override Result ValidateNotNull(DeleteUserRequest value)
    {
        throw new NotImplementedException();
    }
}
