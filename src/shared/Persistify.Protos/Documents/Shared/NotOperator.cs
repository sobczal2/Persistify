using System.Runtime.Serialization;

namespace Persistify.Protos.Documents.Shared;

[DataContract]
public class NotOperator
{
    public NotOperator()
    {
        AndOperator = null;
        OrOperator = null;
        FtsOperator = null;
        NumberOperator = null;
        BoolOperator = null;
    }

    [DataMember(Order = 1)] public AndOperator? AndOperator { get; set; }

    [DataMember(Order = 2)] public OrOperator? OrOperator { get; set; }

    [DataMember(Order = 4)] public FtsQuery? FtsOperator { get; set; }

    [DataMember(Order = 5)] public NumberQuery? NumberOperator { get; set; }

    [DataMember(Order = 6)] public BoolQuery? BoolOperator { get; set; }
}
