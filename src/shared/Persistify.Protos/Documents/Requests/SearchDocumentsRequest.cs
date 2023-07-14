using System.Runtime.Serialization;
using Persistify.Protos.Common;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Protos.Documents.Requests;

[DataContract]
public class SearchDocumentsRequest
{
    public SearchDocumentsRequest()
    {
        TemplateName = null!;
        AndOperator = null;
        OrOperator = null;
        NotOperator = null;
        FtsQuery = null;
        NumberQuery = null;
        BoolQuery = null;
        Pagination = null!;
    }

    [DataMember(Order = 1)] public string TemplateName { get; set; }

    [DataMember(Order = 2)] public AndOperator? AndOperator { get; set; }

    [DataMember(Order = 3)] public OrOperator? OrOperator { get; set; }

    [DataMember(Order = 4)] public NotOperator? NotOperator { get; set; }

    [DataMember(Order = 5)] public FtsQuery? FtsQuery { get; set; }

    [DataMember(Order = 6)] public NumberQuery? NumberQuery { get; set; }

    [DataMember(Order = 7)] public BoolQuery? BoolQuery { get; set; }

    [DataMember(Order = 8)] public Pagination Pagination { get; set; }
}
