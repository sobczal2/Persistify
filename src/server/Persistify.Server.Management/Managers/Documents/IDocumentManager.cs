using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Dtos.Documents.Search;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Server.Domain.Documents;

namespace Persistify.Server.Management.Managers.Documents;

public interface IDocumentManager : IManager
{
    ValueTask<Document?> GetAsync(
        int id
    );

    ValueTask<bool> ExistsAsync(
        int id
    );

    IAsyncEnumerable<Document> ListAsync(
        int take,
        int skip
    );

    ValueTask<(List<SearchRecordDto> searchRecords, int count)> SearchAsync(
        SearchQueryDto searchQueryDto,
        int take,
        int skip
    );

    int Count();

    void Add(
        Document document
    );

    ValueTask<bool> RemoveAsync(
        int id
    );
}
