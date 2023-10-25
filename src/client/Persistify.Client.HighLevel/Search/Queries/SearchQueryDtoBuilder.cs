using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search.Queries.Aggregates;
using Persistify.Client.HighLevel.Search.Queries.Bool;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Helpers.Results;

namespace Persistify.Client.HighLevel.Search.Queries;

public class SearchQueryDtoBuilder<TDocument>
    where TDocument : class
{
    protected IPersistifyHighLevelClient PersistifyHighLevelClient;
    protected SearchQueryDto? SearchQueryDto;

    public SearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
    {
        PersistifyHighLevelClient = persistifyHighLevelClient;
    }

    public AndSearchQueryBuilder<TDocument> And()
    {
        return new AndSearchQueryBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public ExactBoolSearchQueryDtoBuilder<TDocument> ExactBool()
    {
        return new ExactBoolSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    internal SearchQueryDto Build()
    {
        return SearchQueryDto ?? throw new PersistifyHighLevelClientException("Search query is not set");
    }
}
