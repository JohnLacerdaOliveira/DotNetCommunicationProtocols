using ProtoBuf;

namespace GrpcAspNet.Contracts.Requests;

[ProtoContract]
public class JoinRequest
{
    [ProtoMember(1)]
    public string Username { get; set; } = "";
}
