using System;
using ProtoBuf;

namespace Persistify.Server.Domain.Documents;

[ProtoContract]
public class DateFieldValue : FieldValue
{
    [ProtoMember(1)]
    public DateTime Value { get; set; }
}
