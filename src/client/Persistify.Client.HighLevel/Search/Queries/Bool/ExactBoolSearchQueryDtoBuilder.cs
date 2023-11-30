using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.Bool;

namespace Persistify.Client.HighLevel.Search.Queries.Bool;

public class ExactBoolSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, ExactBoolSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public ExactBoolSearchQueryDtoBuilder(
        IPersistifyHighLevelClient persistifyHighLevelClient
    )
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new ExactBoolSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Bool;

    public ExactBoolSearchQueryDtoBuilder<TDocument> WithValue(
        bool value
    )
    {
        ((ExactBoolSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
