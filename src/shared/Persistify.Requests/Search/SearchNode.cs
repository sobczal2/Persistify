using ProtoBuf;

namespace Persistify.Requests.Search;

[ProtoContract]
[ProtoInclude(1, typeof(AndSearchNode))]
[ProtoInclude(2, typeof(OrSearchNode))]
[ProtoInclude(3, typeof(NotSearchNode))]
[ProtoInclude(4, typeof(TextSearchNode))]
[ProtoInclude(5, typeof(FtsSearchNode))]
[ProtoInclude(6, typeof(NumberSearchNode))]
[ProtoInclude(7, typeof(NumberRangeSearchNode))]
[ProtoInclude(8, typeof(BoolSearchNode))]
public class SearchNode
{
}
