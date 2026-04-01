using ProtoBuf;

namespace Contracts.Requests;

[ProtoContract]
public class SubscribeRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = "";
}
