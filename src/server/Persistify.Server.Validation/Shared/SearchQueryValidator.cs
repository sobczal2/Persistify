using System.Threading.Tasks;
using Persistify.Domain.Search.Queries;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class SearchQueryValidator : Validator<SearchQuery>
{
    public override ValueTask<Result> ValidateNotNullAsync(SearchQuery value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
