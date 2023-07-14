using System;
using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class OrOperator
{
    public OrOperator()
    {
        AndOperators = Array.Empty<AndOperator>();
        NotOperators = Array.Empty<NotOperator>();
        FtsQueries = Array.Empty<FtsQuery>();
        NumberQueries = Array.Empty<NumberQuery>();
        BoolQueries = Array.Empty<BoolQuery>();
    }

    [DataMember(Order = 1)] public AndOperator[] AndOperators { get; set; }

    [DataMember(Order = 2)] public NotOperator[] NotOperators { get; set; }

    [DataMember(Order = 4)] public FtsQuery[] FtsQueries { get; set; }

    [DataMember(Order = 5)] public NumberQuery[] NumberQueries { get; set; }

    [DataMember(Order = 6)] public BoolQuery[] BoolQueries { get; set; }
}
