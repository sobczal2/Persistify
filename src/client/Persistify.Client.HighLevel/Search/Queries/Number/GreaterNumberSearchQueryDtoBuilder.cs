using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Search.Queries.Common;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.Search.Queries.Number;

namespace Persistify.Client.HighLevel.Search.Queries.Number;

public class GreaterNumberSearchQueryDtoBuilder<TDocument>
    : FieldSearchQueryDtoBuilder<TDocument, GreaterNumberSearchQueryDtoBuilder<TDocument>>
    where TDocument : class
{
    public GreaterNumberSearchQueryDtoBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
        : base(persistifyHighLevelClient)
    {
        SearchQueryDto = new GreaterNumberSearchQueryDto { Boost = 1 };
    }

    protected override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public GreaterNumberSearchQueryDtoBuilder<TDocument> WithValue(double value)
    {
        ((GreaterNumberSearchQueryDto)SearchQueryDto!).Value = value;
        return this;
    }
}
