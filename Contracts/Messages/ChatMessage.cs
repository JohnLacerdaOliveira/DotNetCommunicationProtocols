using ProtoBuf;

namespace Contracts.Messages;

[ProtoContract]
public class ChatMessage
{
    [ProtoMember(1)]
    public string User { get; set; } = "";

    [ProtoMember(2)]
    public string Message { get; set; } = "";

    [ProtoMember(3)]
    public DateTime Timestamp { get; set; }

    [ProtoMember(4)]
    public MessageType Type { get; set; } = MessageType.Chat;
}