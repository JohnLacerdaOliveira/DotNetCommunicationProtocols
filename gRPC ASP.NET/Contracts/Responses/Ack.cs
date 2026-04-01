using ProtoBuf;

namespace GrpcAspNet.Contracts.Responses;

[ProtoContract]
public class Ack
{
    [ProtoMember(1)]
    public int Count { get; set; }
}