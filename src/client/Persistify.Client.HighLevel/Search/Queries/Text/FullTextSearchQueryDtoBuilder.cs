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

public class FullTextSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, FullTextSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public FullTextSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new FullTextSearchQueryDto { Boost = 1 };
    }

    public FullTextSearchQueryDtoBuilder<TDocument> WithValue(string value)
    {
        ((FullTextSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
}
