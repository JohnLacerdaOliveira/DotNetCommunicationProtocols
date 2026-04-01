using ProtoBuf;

namespace GrpcAspNet.Contracts.Messages;

[ProtoContract]
public enum MessageType
{
    [ProtoEnum]
    Chat = 0,

    [ProtoEnum]
    Join = 1,

    [ProtoEnum]
    Leave = 2
}