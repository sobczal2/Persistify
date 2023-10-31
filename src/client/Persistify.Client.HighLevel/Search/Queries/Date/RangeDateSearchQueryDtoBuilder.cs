using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.Date;

public class RangeDateSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, RangeNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public RangeDateSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new RangeDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public RangeDateSearchQueryDtoBuilder<TDocument> WithMinValue(DateTime minValue)
    {
        ((RangeDateTimeSearchQueryDto)SearchQueryDto!).MinValue = minValue;
        return this;
    }

    public RangeDateSearchQueryDtoBuilder<TDocument> WithMaxValue(DateTime maxValue)
    {
        ((RangeDateTimeSearchQueryDto)SearchQueryDto!).MaxValue = maxValue;
        return this;
    }
}
