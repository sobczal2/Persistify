using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.Date;

public class LessDateTimeSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, LessNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public LessDateTimeSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new LessDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public LessDateTimeSearchQueryDtoBuilder<TDocument> WithValue(DateTime value)
    {
        ((LessDateTimeSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
