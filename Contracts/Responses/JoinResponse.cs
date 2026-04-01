using ProtoBuf;

namespace Contracts.Responses;

[ProtoContract]
public class JoinResponse
{
    [ProtoMember(1)]
    public string Message { get; set; } = "";
}
