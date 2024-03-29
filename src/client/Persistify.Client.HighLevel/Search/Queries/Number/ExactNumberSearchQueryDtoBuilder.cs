﻿using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.Number;

namespace Persistify.Client.HighLevel.Search.Queries.Number;

public class ExactNumberSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, ExactNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public ExactNumberSearchQueryDtoBuilder(
        IPersistifyHighLevelClient persistifyHighLevelClient
    )
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new ExactNumberSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public ExactNumberSearchQueryDtoBuilder<TDocument> WithValue(
        double value
    )
    {
        ((ExactNumberSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
