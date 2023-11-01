using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.DateTime;

public class RangeDateTimeSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, RangeNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public RangeDateTimeSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new RangeDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public RangeDateTimeSearchQueryDtoBuilder<TDocument> WithMinValue(System.DateTime minValue)
    {
        ((RangeDateTimeSearchQueryDto)SearchQueryDto!).MinValue = minValue;
        return this;
    }

    public RangeDateTimeSearchQueryDtoBuilder<TDocument> WithMaxValue(System.DateTime maxValue)
    {
        ((RangeDateTimeSearchQueryDto)SearchQueryDto!).MaxValue = maxValue;
        return this;
    }
}
