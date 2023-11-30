using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.DateTime;

public class GreaterDateTimeSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, GreaterNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public GreaterDateTimeSearchQueryDtoBuilder(
        IPersistifyHighLevelClient persistifyHighLevelClient
    )
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new GreaterDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public GreaterDateTimeSearchQueryDtoBuilder<TDocument> WithValue(
        System.DateTime value
    )
    {
        ((GreaterDateTimeSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
