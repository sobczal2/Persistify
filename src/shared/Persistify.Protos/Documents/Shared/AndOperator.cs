using System;
using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class AndOperator
{
    public AndOperator()
    {
        OrOperators = Array.Empty<OrOperator>();
        NotOperators = Array.Empty<NotOperator>();
        FtsQueries = Array.Empty<FtsQuery>();
        NumberQueries = Array.Empty<NumberQuery>();
        BoolQueries = Array.Empty<BoolQuery>();
    }

    [DataMember(Order = 1)] public OrOperator[] OrOperators { get; set; } = default!;

    [DataMember(Order = 2)] public NotOperator[] NotOperators { get; set; } = default!;

    [DataMember(Order = 4)] public FtsQuery[] FtsQueries { get; set; } = default!;

    [DataMember(Order = 5)] public NumberQuery[] NumberQueries { get; set; } = default!;

    [DataMember(Order = 6)] public BoolQuery[] BoolQueries { get; set; } = default!;
}
