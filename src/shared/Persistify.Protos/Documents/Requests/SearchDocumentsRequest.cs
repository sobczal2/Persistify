using System.Runtime.Serialization;
using Persistify.Protos.Common;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class SearchDocumentsRequest
{
    [DataMember(Order = 1)] public string TemplateName { get; set; } = default!;

    [DataMember(Order = 2)] public AndOperator? AndOperator { get; set; }

    [DataMember(Order = 3)] public OrOperator? OrOperator { get; set; }

    [DataMember(Order = 4)] public NotOperator? NotOperator { get; set; }

    [DataMember(Order = 5)] public FtsQuery? FtsOperator { get; set; }

    [DataMember(Order = 6)] public NumberQuery? NumberOperator { get; set; }

    [DataMember(Order = 7)] public BoolQuery? BoolOperator { get; set; }

    [DataMember(Order = 8)] public Pagination Pagination { get; set; } = default!;
}
