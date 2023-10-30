using System;
using System.Linq;
using System.Linq.Expressions;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.Number;

namespace Persistify.Client.HighLevel.Search.Queries.Number;

public class RangeNumberSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, RangeNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public RangeNumberSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new RangeNumberSearchQueryDto { Boost = 1 };
    }

    public RangeNumberSearchQueryDtoBuilder<TDocument> WithMinValue(double minValue)
    {
        ((RangeNumberSearchQueryDto)SearchQueryDto!).MinValue = minValue;
        return this;
    }

    public RangeNumberSearchQueryDtoBuilder<TDocument> WithMaxValue(double maxValue)
    {
        ((RangeNumberSearchQueryDto)SearchQueryDto!).MaxValue = maxValue;
        return this;
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;
}
