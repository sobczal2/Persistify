using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class EmptyRequestValidator : Validator<EmptyRequest>
{
    public EmptyRequestValidator()
    {
        PropertyName.Push(nameof(EmptyRequest));
    }

    public override Result ValidateNotNull(EmptyRequest value)
    {
        return Result.Ok;
    }
}
