using System;
using System.Linq;
using System.Linq.Expressions;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.Text;

namespace Persistify.Client.HighLevel.Search.Queries.Text;

public class ExactTextSearchQueryDtoBuilder<TDocument> : FieldSearchQueryDtoBuilder<TDocument, ExactTextSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public ExactTextSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient) : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new ExactTextSearchQueryDto { Boost = 1 };
    }

    public ExactTextSearchQueryDtoBuilder<TDocument> WithValue(string value)
    {
        ((ExactTextSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
}
