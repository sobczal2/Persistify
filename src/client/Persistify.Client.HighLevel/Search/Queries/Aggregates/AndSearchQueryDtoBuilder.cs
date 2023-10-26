using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;
using Persistify.Helpers.Results;

namespace Persistify.Client.HighLevel.Search.Queries.Aggregates;

public class AndSearchQueryBuilder<TDocument> : SearchQueryDtoBuilder<TDocument>
    where TDocument : class
{
    public AndSearchQueryBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new AndSearchQueryDto { Boost = 1 };
    }

    public AndSearchQueryBuilder<TDocument> AddQuery(Func<SearchQueryDtoBuilder<TDocument>, SearchQueryDtoBuilder<TDocument>> searchQueryAction)
    {
        var searchQueryBuilder = new SearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
        ((AndSearchQueryDto)SearchQueryDto!).SearchQueryDtos.Add(searchQueryAction(searchQueryBuilder).Build());
        return this;
    }

    public AndSearchQueryBuilder<TDocument> ClearQueries()
    {
        ((AndSearchQueryDto)SearchQueryDto!).SearchQueryDtos.Clear();
        return this;
    }

    public AndSearchQueryBuilder<TDocument> WithBoost(float boost)
    {
        ((AndSearchQueryDto)SearchQueryDto!).Boost = boost;
        return this;
    }
}
