using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.Date;

public class LessDateSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, LessNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public LessDateSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new LessDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public LessDateSearchQueryDtoBuilder<TDocument> WithValue(DateTime value)
    {
        ((LessDateTimeSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
