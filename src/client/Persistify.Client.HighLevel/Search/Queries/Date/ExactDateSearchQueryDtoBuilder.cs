using System;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Client.HighLevel.Search.Queries.Number;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.DateTime;

namespace Persistify.Client.HighLevel.Search.Queries.Date;

public class ExactDateSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, ExactNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public ExactDateSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new ExactDateTimeSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.DateTime;

    public ExactDateSearchQueryDtoBuilder<TDocument> WithValue(DateTime value)
    {
        ((ExactDateTimeSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
