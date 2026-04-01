using ProtoBuf;

namespace GrpcAspNet.Contracts.Requests;

[ProtoContract]
public class SubscribeRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = "";
}
