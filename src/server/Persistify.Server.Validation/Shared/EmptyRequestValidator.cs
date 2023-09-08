using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class EmptyRequestValidator : IValidator<EmptyRequest>
{
    public EmptyRequestValidator()
    {
        ErrorPrefix = "EmptyRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(EmptyRequest value)
    {
        return Result.Ok;
    }
}
