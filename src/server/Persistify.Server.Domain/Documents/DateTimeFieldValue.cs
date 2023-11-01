using System;
using ProtoBuf;

namespace Persistify.Server.Domain.Documents;

[ProtoContract]
public class DateTimeFieldValue : FieldValue
{
    [ProtoMember(2)]
    public DateTime Value { get; set; }
}
