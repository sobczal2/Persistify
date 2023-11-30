using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;

namespace Persistify.Client.HighLevel.Search.Queries.Aggregates;

public class OrSearchQueryDtoBuilder<TDocument> : SearchQueryDtoBuilder<TDocument>
    where TDocument : class
{
    public OrSearchQueryDtoBuilder(
        IPersistifyHighLevelClient persistifyHighLevelClient
    )
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new OrSearchQueryDto { Boost = 1 };
    }

    public OrSearchQueryDtoBuilder<TDocument> AddQuery(
        Func<SearchQueryDtoBuilder<TDocument>, SearchQueryDtoBuilder<TDocument>> searchQueryAction
    )
    {
        var searchQueryBuilder = new SearchQueryDtoBuilder<TDocument>(PersistifyHighLevelClient);
        ((OrSearchQueryDto)SearchQueryDto!).SearchQueryDtos.Add(
            searchQueryAction(searchQueryBuilder).Build()
        );
        return this;
    }

    public OrSearchQueryDtoBuilder<TDocument> ClearQueries()
    {
        ((OrSearchQueryDto)SearchQueryDto!).SearchQueryDtos.Clear();
        return this;
    }

    public OrSearchQueryDtoBuilder<TDocument> WithBoost(
        float boost
    )
    {
        ((OrSearchQueryDto)SearchQueryDto!).Boost = boost;
        return this;
    }
}
