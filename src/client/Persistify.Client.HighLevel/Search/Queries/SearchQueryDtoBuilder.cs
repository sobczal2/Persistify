using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search.Queries.Aggregates;
using Persistify.Client.HighLevel.Search.Queries.Bool;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Client.HighLevel.Search.Queries.Text;
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

    public OrSearchQueryDtoBuilder<TDocument> Or()
    {
        return new OrSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public ExactBoolSearchQueryDtoBuilder<TDocument> ExactBool()
    {
        return new ExactBoolSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public ExactNumberSearchQueryDtoBuilder<TDocument> ExactNumber()
    {
        return new ExactNumberSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public GreaterNumberSearchQueryDtoBuilder<TDocument> GreaterNumber()
    {
        return new GreaterNumberSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public LessNumberSearchQueryDtoBuilder<TDocument> LessNumber()
    {
        return new LessNumberSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public RangeNumberSearchQueryDtoBuilder<TDocument> RangeNumber()
    {
        return new RangeNumberSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public ExactTextSearchQueryDtoBuilder<TDocument> ExactText()
    {
        return new ExactTextSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public FullTextSearchQueryDtoBuilder<TDocument> FullText()
    {
        return new FullTextSearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
    }

    public SearchQueryDtoBuilder<TDocument> WithBoost(float boost)
    {
        SearchQueryDto!.Boost = boost;
        return this;
    }

    internal SearchQueryDto Build()
    {
        return SearchQueryDto
            ?? throw new PersistifyHighLevelClientException("Search query is not set");
    }
}
