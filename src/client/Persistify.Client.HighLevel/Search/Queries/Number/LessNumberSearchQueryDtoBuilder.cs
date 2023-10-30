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

public class LessNumberSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, LessNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public LessNumberSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new LessNumberSearchQueryDto { Boost = 1 };
    }

    public LessNumberSearchQueryDtoBuilder<TDocument> WithValue(double value)
    {
        ((LessNumberSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;
}
